using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using System.Globalization;
using ImGuiNET;
using System;
using System.Linq;
using EurekaHelper.XIV;
using EurekaHelper.XIV.Zones;
using Dalamud.Interface;
using System.Diagnostics;
using System.Reflection;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Collections.Generic;

namespace EurekaHelper
{
    internal static class Constants
    {
        public static readonly string EurekaTrackerLink = "https://ffxiv-eureka.com/";

        // 732 - Anemos
        // 763 - Pagos
        // 795 - Pyros
        // 827 - Hydatos
        public static readonly ushort[] EurekaZones = { 732, 763, 795, 827 };

        public static readonly ushort[] BunnyFates = { 1367, 1368, 1407, 1408, 1425 };

        public static readonly float RandomizeRange = 0.5f;

        // Only allow these datacenters, the rest are not supported.
        public static readonly Dictionary<int, string> DatacenterToEurekaDataCenterId = new()
        {
            { 1, "Elemental" },
            { 2, "Gaia" },
            { 3, "Mana" },
            { 4, "Aether" },
            { 5, "Primal" },
            { 6, "Chaos" },
            { 10, "Crystal" },
            { 11, "Light" },
            { 12, "Materia" },
            { 13, "Dynamis" },
            { 14, "Meteor" }
        };
    }

    internal class Utils
    {
        private static readonly Random random = new();

        public static bool IsPlayerInEurekaZone(ushort territoryId) => Constants.EurekaZones.Contains(territoryId);

        public static bool IsBunnyFate(ushort fateId) => Constants.BunnyFates.Contains(fateId);

        public static int GetIndexOfZone(ushort id) => Array.IndexOf(Constants.EurekaZones, id) + 1;

        public static string CombineUrl(string baseUrl, params string[] segments) => string.Join("/", new[] { baseUrl.TrimEnd('/') }.Concat(segments.Select(s => s.Trim('/'))));

        public static IEurekaTracker GetEurekaTracker(ushort territoryId)
        {
            return territoryId switch
            {
                732 or 1 => EurekaAnemos.GetTracker(),
                763 or 2 => EurekaPagos.GetTracker(),
                795 or 3 => EurekaPyros.GetTracker(),
                827 or 4 or _ => EurekaHydatos.GetTracker(),
            };
        }

        public static void CopyToClipboard(string message) => ImGui.SetClipboardText(message);

        public static SeString MapLink(uint territoryId, uint mapId, Vector2 position)
        {
            var mapPayload = new MapLinkPayload(territoryId, mapId, position.X, position.Y);
            var name = $"{mapPayload.PlaceName} ({position.X.ToString("00.0", CultureInfo.InvariantCulture)}, {position.Y.ToString("00.0", CultureInfo.InvariantCulture)})";
            return new SeStringBuilder()
                .AddUiForeground(0x0225)
                .AddUiGlow(0x0226)
                .Add(mapPayload)
                .AddUiForeground(500)
                .AddUiGlow(501)
                .AddText($"{(char)SeIconChar.LinkMarker}")
                .AddUiGlowOff()
                .AddUiForegroundOff()
                .AddText(name)
                .Add(RawPayload.LinkTerminator)
                .AddUiGlowOff()
                .AddUiForegroundOff()
                .BuiltString;
        }

        public static void TextURL(string name, string url, uint color)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.Text(name);
            ImGui.PopStyleColor();

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }

                DrawUnderline(ImGui.GetColorU32(ImGuiCol.ButtonHovered));
                ImGui.BeginTooltip();
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.Text(FontAwesomeIcon.Link.ToIconString()); ImGui.SameLine(0.0f, ImGui.GetStyle().ItemInnerSpacing.X);
                ImGui.PopFont();
                ImGui.Text(url);
                ImGui.EndTooltip();
            }
            else
            {
                DrawUnderline(ImGui.GetColorU32(ImGuiCol.Button));
            }
        }

        public static void DrawUnderline(uint color)
        {
            var min = ImGui.GetItemRectMin();
            var max = ImGui.GetItemRectMax();
            min.Y = max.Y;
            ImGui.GetWindowDrawList().AddLine(min, max, color, 1.0f);
        }

        public static void SetTooltip(string message)
        {
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(message);
        }

        public static void CenterText(string text)
        {
            var result = Vector2.Subtract(ImGui.GetContentRegionAvail(), ImGui.CalcTextSize(text));
            ImGui.SetCursorPos(new Vector2(result.X / 2, result.Y / 2));
            ImGui.Text(text);
        }

        public static void RightAlignTextInColumn(string text, Vector4? color = null)
        {
            var posX = (ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(text).X - ImGui.GetScrollX());
            if (posX > ImGui.GetCursorPosX())
                ImGui.SetCursorPosX(posX);

            if (color == null)
                ImGui.Text(text);
            else
                ImGui.TextColored((Vector4)color, text);
        }

        public static unsafe void SetFlagMarker(EurekaFate fateInfo, bool openMap = false, bool randomizeCoords = false)
        {
            var instance = AgentMap.Instance();

            var XValue = randomizeCoords ? GetRandomizeFloat(fateInfo.FatePosition.X) : fateInfo.FatePosition.X;
            var YValue = randomizeCoords ? GetRandomizeFloat(fateInfo.FatePosition.Y) : fateInfo.FatePosition.Y;

            if (instance != null)
            {
                var mapPayload = new MapLinkPayload(fateInfo.TerritoryId, fateInfo.MapId, XValue, YValue);
                instance->IsFlagMarkerSet = 0;
                instance->SetFlagMapMarker(mapPayload.Map.TerritoryType.Row, mapPayload.Map.RowId, mapPayload.RawX / 1000f, mapPayload.RawY / 1000f);

                if (openMap)
                    instance->OpenMap(mapPayload.Map.RowId, mapPayload.Map.TerritoryType.Row);
            }
        }

        public static float GetRandomizeFloat(float centerValue) => (float)(centerValue + (random.NextDouble() * Constants.RandomizeRange * 2.0f - Constants.RandomizeRange));

        public static void SendMessage(string message)
        {
            var sanitized = DalamudApi.XivCommonBase.Functions.Chat.SanitiseText(message);
            DalamudApi.XivCommonBase.Functions.Chat.SendMessage(sanitized);
        }

        public static int DatacenterToEurekaDatacenterId(string datacenterName) => Constants.DatacenterToEurekaDataCenterId.FirstOrDefault(x => x.Value == datacenterName).Key;

        public static string RandomFormattedText(EurekaFate fate)
        {
            // Select a random custom message
            int index = random.Next(EurekaHelper.Config.CustomMessages.Count);
            string randomMessage = EurekaHelper.Config.CustomMessages[index];

            return randomMessage
                .Replace("%bossName%", fate.BossName)
                .Replace("%bossShortName%", fate.BossShortName)
                .Replace("%fateName%", fate.FateName)
                .Replace("%flag%", "<flag>");
        }

        public static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unable to get version";

        public static string GetGitSha() => Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unable to get Git Hash";
    }
}
