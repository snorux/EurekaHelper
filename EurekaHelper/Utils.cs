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
using Lumina.Data.Parsing.Layer;
using Lumina.Excel.GeneratedSheets;
using Lumina.Data.Files;
using Dalamud.Utility;
using Dalamud.Logging;

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

        public static readonly uint[] EurekaElementals = { 7184, 7567, 7764, 8131 };

        public static readonly float RandomizeRange = 0.5f;

        public static readonly int MapCircleRadius = 80;

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

        private static readonly Dictionary<int, List<LayerCommon.InstanceObject>> LgbEventData = new();

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

        public static string GetZoneName(ushort zoneId)
        {
            return zoneId switch
            {
                732 => "Anemos",
                763 => "Pagos",
                795 => "Pyros",
                827 => "Hydatos",
                _ => null
            };
        }

        public static void BuildLgbData()
        {
            foreach (var zone in Constants.EurekaZones)
            {
                List<LayerCommon.InstanceObject> eventData = new();

                var territoryType = DalamudApi.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(zone);

                if (territoryType == null)
                    continue;

                var bg = territoryType.Bg.ToString();
                if (string.IsNullOrWhiteSpace(bg))
                    continue;

                var lgbFileName = "bg/" + bg[..(bg.IndexOf("/level/") + 1)] + "level/planevent.lgb";
                var lgbFile = DalamudApi.DataManager.GetFile<LgbFile>(lgbFileName);

                if (lgbFile == null)
                    continue;

                foreach (var lgbGroup in lgbFile.Layers)
                {
                    foreach (var instanceObject in lgbGroup.InstanceObjects)
                    {
                        if (instanceObject.AssetType == LayerEntryType.EventRange)
                            eventData.Add(instanceObject);
                    }
                }

                LgbEventData.Add(zone, eventData);
            }
        }

        public static void GetFatePositionFromLgb(ushort territoryId, List<EurekaFate> fates)
        {
            if (!LgbEventData.TryGetValue(territoryId, out var eventData))
                return;

            var territoryType = DalamudApi.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(territoryId);

            if (territoryType == null)
                return;

            foreach (var fate in fates)
            {
                var fateSheetData = DalamudApi.DataManager.GetExcelSheet<Fate>()!.GetRow(fate.FateId);
                if (fateSheetData.Location == 0)
                    continue;

                if (!eventData.Exists(x => x.InstanceId == fateSheetData.Location))
                    continue;

                var match = eventData.Find(x => x.InstanceId == fateSheetData.Location);

                var vector = MapUtil.WorldToMap(new Vector2(match.Transform.Translation.X, match.Transform.Translation.Z), territoryType.Map.Value);

                fate.FatePosition = vector;
            }
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

        public static SeString ItemLink(uint itemId, bool isHq = false, string? displayNameOverride = null)
        {
            var itemLink = SeString.CreateItemLink(itemId, isHq, displayNameOverride);
            return new SeStringBuilder()
                .AddText("Linked Item: ")
                .Append(itemLink)
                .Add(RawPayload.LinkTerminator)
                .BuiltString;
        }

        public static SeStringBuilder ArisuStringbuilder(string nextNmString, string nextWeatherString, DateTime time1, DateTime time2)
        {
            var sb = new SeStringBuilder();
            var nextTimeOfWeather = time1;
            if (time1 < DateTime.Now)
            {
                var currTimeDiff = time1 + TimeSpan.FromMilliseconds(EorzeaTime.EIGHT_HOURS) - DateTime.Now;
                sb.AddUiForeground(523)
                    .AddText($"{nextNmString} weather is up now! It ends in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(currTimeDiff.TotalMinutes)}m. ");
                nextTimeOfWeather = time2;
            }

            var nextTimeDiff = nextTimeOfWeather - DateTime.Now;
            sb.AddUiForeground(523)
                .AddText($"Next {nextNmString} weather ({nextWeatherString}) in ")
                .AddUiForegroundOff()
                .AddUiForeground(508)
                .AddText($"{(int)Math.Round(nextTimeDiff.TotalMinutes)}m ")
                .AddUiForegroundOff()
                .AddUiForeground(523)
                .AddText("@ ")
                .AddUiForegroundOff()
                .AddUiForeground(508)
                .AddText($"{nextTimeOfWeather:d MMM yyyy hh:mm tt}")
                .AddUiForegroundOff();
            return sb;
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
            var posX = ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(text).X - ImGui.GetScrollX();
            if (posX > ImGui.GetCursorPosX())
                ImGui.SetCursorPosX(posX);

            if (color == null)
                ImGui.Text(text);
            else
                ImGui.TextColored((Vector4)color, text);
        }

        public static unsafe void SetFlagMarker(ushort territoryId, ushort mapId, Vector2 position, bool openMap = false, bool randomizeCoords = false, bool drawCircle = false)
        {
            var instance = AgentMap.Instance();

            var PosX = randomizeCoords ? GetRandomizeFloat(position.X) : position.X;
            var PosY = randomizeCoords ? GetRandomizeFloat(position.Y) : position.Y;

            if (instance != null)
            {
                var mapPayload = new MapLinkPayload(territoryId, mapId, PosX, PosY);
                instance->IsFlagMarkerSet = 0;

                if (drawCircle)
                {
                    instance->TempMapMarkerCount = 0;
                    instance->AddGatheringTempMarker(mapPayload.RawX / 1000, mapPayload.RawY / 1000, Constants.MapCircleRadius);
                }

                instance->SetFlagMapMarker(mapPayload.Map.TerritoryType.Row, mapPayload.Map.RowId, mapPayload.RawX / 1000f, mapPayload.RawY / 1000f);

                if (openMap)
                    instance->OpenMap(mapPayload.Map.RowId, mapPayload.Map.TerritoryType.Row, type: drawCircle ? FFXIVClientStructs.FFXIV.Client.UI.Agent.MapType.GatheringLog : FFXIVClientStructs.FFXIV.Client.UI.Agent.MapType.FlagMarker);
            }
        }

        public static void SetFlagMarker(EurekaFate fateInfo, bool openMap = false, bool randomizeCoords = false, bool drawCircle = false)
            => SetFlagMarker(fateInfo.TerritoryId, fateInfo.MapId, new Vector2(fateInfo.FatePosition.X, fateInfo.FatePosition.Y), openMap, randomizeCoords, drawCircle);

        public static unsafe void AddMapMarker(ushort territoryId, Vector3 position, uint iconId, bool openMap = false)
        {
            if (DalamudApi.ClientState.TerritoryType != territoryId)
            {
                EurekaHelper.PrintMessage("You must be in the same zone to place a marker.");
                return;
            }

            var instance = AgentMap.Instance();

            if (instance != null)
            {
                var territoryType = DalamudApi.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(territoryId);
                var mapPosition = position;
                mapPosition.X += territoryType.Map.Value.OffsetX;
                mapPosition.Z += territoryType.Map.Value.OffsetY;

                if (!instance->AddMapMarker(mapPosition, iconId))
                    PluginLog.Error("Unable to place map marker");

                if (!instance->AddMiniMapMarker(position, iconId))
                    PluginLog.Error("Unable to place mini map marker");

                if (openMap)
                    instance->OpenMap(territoryType.Map.Value.RowId, territoryType.RowId);
            }
        }

        public static unsafe void ClearMapMarker()
        {
            var instance = AgentMap.Instance();

            if (instance != null)
            {
                instance->ResetMapMarkers();
                instance->ResetMiniMapMarkers();
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

        public static bool IsWithinMinimumDistance(Vector3 position, Vector3 targetPosition, float minDistance)
        {
            float distance = Vector3.Distance(position, targetPosition);
            return distance < minDistance;
        }

        public static string GetItemName(uint itemId)
        {
            return DalamudApi.DataManager.Excel.GetSheet<Item>()!
                .GetRow(itemId)!.Name
                .ToDalamudString()
                .ToString();
        }

        public static string GetJobCategory(uint itemId)
        {
            return DalamudApi.DataManager.Excel.GetSheet<Item>()!
                .GetRow(itemId)!.ClassJobCategory
                .Value
                .Name
                .ToDalamudString()
                .ToString();
        }

        public static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unable to get version";

        public static string GetGitSha() => Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unable to get Git Hash";
    }
}
