using Dalamud.Interface.Windowing;
using System;
using ImGuiNET;
using System.Numerics;
using Dalamud.Interface;
using EurekaHelper.System;
using System.Threading.Tasks;
using EurekaHelper.XIV;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Logging;
using Dalamud.Interface.Components;
using Dalamud.Game.Text;
using Dalamud.Interface.Colors;

namespace EurekaHelper.Windows
{
    public class PluginWindow : Window, IDisposable
    {
        private readonly EurekaHelper Plugin = null!;

        public PluginWindow(EurekaHelper plugin) : base("Eureka Helper")
        {
            Plugin = plugin;
            SizeConstraints = new WindowSizeConstraints { MinimumSize = new Vector2(566, 520), MaximumSize = new Vector2(float.MaxValue, float.MaxValue) };
        }

        private static EurekaConnectionManager Connection = new();

        public void Dispose() { }

        public override void Draw()
        {
            if (ImGui.BeginTabBar("EHelperTab"))
            {
                if (ImGui.BeginTabItem("Tracker"))
                {
                    DrawTrackerTab();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Elementals"))
                {
                    DrawElementalTab();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Configuration"))
                {
                    DrawSettingsTab();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Instance"))
                {
                    DrawInstanceTab();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("About"))
                {
                    DrawAboutTab();
                    ImGui.EndTabItem();
                }
            }
        }

        public string TrackerCode = string.Empty;
        public string TrackerPassword = string.Empty;

        public async void DrawTrackerTab()
        {
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Settings:"); ImGui.SameLine();

            ImGui.SameLine();

            if (!Connection.IsConnected())
            {
                if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
                    ImGui.OpenPopup("CreateTracker");
                Utils.SetTooltip("Create a new tracker");

                ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
                if (ImGui.BeginPopup("CreateTracker"))
                {
                    if (ImGui.Button("Create Anemos Tracker"))
                    {
                        _ = Task.Run(async () => { await CreateTracker(1); });
                        ImGui.CloseCurrentPopup();
                    }

                    if (ImGui.Button("Create Pagos Tracker"))
                    {
                        _ = Task.Run(async () => { await CreateTracker(2); });
                        ImGui.CloseCurrentPopup();
                    }

                    if (ImGui.Button("Create Pyros Tracker"))
                    {
                        _ = Task.Run(async () => { await CreateTracker(3); });
                        ImGui.CloseCurrentPopup();
                    }

                    if (ImGui.Button("Create Hydatos Tracker"))
                    {
                        _ = Task.Run(async () => { await CreateTracker(4); });
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }
                ImGui.PopStyleVar();
                ImGui.PopStyleColor();
            }

            else if (Connection.IsConnected())
            {
                if (ImGuiComponents.IconButton(FontAwesomeIcon.Link))
                    Utils.CopyToClipboard($"{Utils.CombineUrl(Constants.EurekaTrackerLink, Connection.GetTrackerId())}");
                Utils.SetTooltip("Copy tracker link to clipboard");

                if (Connection.CanModify())
                {
                    ImGui.SameLine();

                    if (ImGuiComponents.IconButton(FontAwesomeIcon.Key))
                        Utils.CopyToClipboard($"Password: {Connection.GetTrackerPassword()}");
                    Utils.SetTooltip("Copy tracker password to clipboard");

                    ImGui.SameLine();

                    if (Connection.IsPublic())
                    {
                        if (ImGuiComponents.IconButton(FontAwesomeIcon.Lock))
                            await Connection.SetTrackerVisiblity();

                        Utils.SetTooltip("Set tracker to private");
                    }
                    else
                    {
                        if (ImGuiComponents.IconButton(FontAwesomeIcon.LockOpen))
                        {
                            var datacenterId = Utils.DatacenterToEurekaDatacenterId(DalamudApi.ClientState.LocalPlayer.CurrentWorld.GameData.DataCenter.Value?.Name.RawString ?? "null");

                            if (datacenterId == 0)
                                EurekaHelper.PrintMessage("This datacenter is not supported currently. Please submit an issue if you think this is incorrect.");
                            else
                                await Connection.SetTrackerVisiblity(datacenterId);
                        }

                        Utils.SetTooltip("Set tracker to public");
                    }
                }

                ImGui.SameLine();

                if (ImGuiComponents.IconButton(FontAwesomeIcon.Globe))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = $"{Constants.EurekaTrackerLink}{Connection.GetTrackerId()}",
                        UseShellExecute = true
                    });
                }
                Utils.SetTooltip("Opens the tracker in a browser");

                ImGui.SameLine();

                if (ImGuiComponents.IconButton(FontAwesomeIcon.FileExport))
                    _ = Task.Run(async () => { await ExportTracker(Connection.GetTrackerId()); });
                Utils.SetTooltip("Exports the current tracker to a new one");

                ImGui.SameLine();

                if (ImGuiComponents.IconButton(FontAwesomeIcon.SignOutAlt))
                {
                    _ = Task.Run(async () =>
                    {
                        await Connection.Close();
                    });
                }
                Utils.SetTooltip("Leave the current tracker");

                ImGui.SameLine();

                ImGuiComponents.IconButton(FontAwesomeIcon.CloudSun);
                if (ImGui.IsItemHovered())
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                    ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));

                    ImGui.BeginTooltip();

                    float spacing = ImGui.GetStyle().ItemInnerSpacing.X;
                    ImGui.Text("E.T:"); ImGui.SameLine(0.0f, spacing);
                    ImGui.TextColored(new Vector4(0.0f, 1.0f, 0.0f, 1.0f), $"{EorzeaTime.Now.EorzeaDateTime:HH:mm}"); ImGui.SameLine(0.0f, spacing);

                    if (EorzeaTime.Now.EorzeaDateTime.Hour < 6 || EorzeaTime.Now.EorzeaDateTime.Hour >= 19)
                    {
                        ImGui.Text("(Night)");
                        ImGui.Text($"Day in {EorzeaTime.Now.TimeUntilDay():mm'm 'ss's'}");
                    }
                    else
                    {
                        ImGui.Text("(Day)");
                        ImGui.Text($"Night in {EorzeaTime.Now.TimeUntilNight():mm'm 'ss's'}");
                    }

                    ImGui.Dummy(new Vector2(0.0f, 10.0f));

                    ImGui.Text("Weather:"); ImGui.SameLine(0.0f, spacing);
                    ImGui.TextColored(new Vector4(0.0f, 1.0f, 0.0f, 1.0f), $"{Connection.GetTracker().GetCurrentWeatherInfo().Weather.ToFriendlyString()}");
                    ImGui.Text($"Ends in {Connection.GetTracker().GetCurrentWeatherInfo().Timeleft:mm'm 'ss's'}");

                    ImGui.Dummy(new Vector2(0.0f, 10.0f));

                    ImGui.Text("Weather Forecast:");
                    var weatherForecast = Connection.GetTracker().GetAllNextWeatherTime();
                    foreach (var (Weather, Time) in weatherForecast)
                    {
                        ImGui.TextColored(new Vector4(1.0f, 0.4f, 1.0f, 1.0f), Weather.ToFriendlyString()); ImGui.SameLine(0.0f, ImGui.GetStyle().ItemInnerSpacing.X);
                        ImGui.Text($"in: {(Time.ToString(Time.Hours > 0 ? "hh'h 'mm'm 'ss's'" : "mm'm 'ss's'"))}");
                    }

                    ImGui.EndTooltip();

                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor();
                }

                ImGui.SameLine();

                ImGuiComponents.IconButton(FontAwesomeIcon.InfoCircle);
                if (ImGui.IsItemHovered())
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                    ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));

                    ImGui.BeginTooltip();

                    ImGui.TextColored(GreenColorText, "Green"); ImGui.SameLine(0.0f, ImGui.GetStyle().ItemInnerSpacing.X);
                    ImGui.Text("=> Ready to be spawned");
                    ImGui.TextColored(RedColorText, "Red"); ImGui.SameLine(0.0f, ImGui.GetStyle().ItemInnerSpacing.X);
                    ImGui.Text("=> Has been popped and is on a respawn timer");
                    ImGui.TextColored(OrangeColorText, "Orange"); ImGui.SameLine(0.0f, ImGui.GetStyle().ItemInnerSpacing.X);
                    ImGui.Text("=> One of the requirements is not met to spawn/prep the NM");

                    ImGui.EndTooltip();

                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor();
                }

                ImGui.SameLine(ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize($"ID: {Connection.GetTrackerId()}\t\tViewers: {Connection.GetViewers()}").X);
                ImGui.AlignTextToFramePadding();
                ImGui.Text($"ID: {Connection.GetTrackerId()}\t\tViewers: {Connection.GetViewers()}");
            }

            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(5.0f, 5.0f));
            if (ImGui.BeginTable("TrackerConnectionSettings", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.NoBordersInBody))
            {
                ImGui.TableSetupColumn("Code", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Password", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Button");

                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Code:"); ImGui.SameLine(); ImGui.SetNextItemWidth(110f);
                ImGui.InputTextWithHint("##TrackerCode", "Enter 6 digit code", ref TrackerCode, 6);

                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Password:"); ImGui.SameLine(); ImGui.SetNextItemWidth(200f);
                ImGui.InputTextWithHint("##TrackerPassword", "Enter tracker password", ref TrackerPassword, 100);
                Utils.SetTooltip("Don't input if you just want to join a tracker.\nIf you have the password, enter the correct password or you'll need to press \"Set\" again.");

                ImGui.TableNextColumn();
                if (ImGui.Button("Set", new Vector2(ImGui.GetContentRegionAvail().X, 0.0f)))
                {
                    if (!string.IsNullOrWhiteSpace(TrackerCode))
                    {
                        _ = Task.Run(async () =>
                        {
                            if (Connection.GetTrackerId() == TrackerCode)
                            {
                                if (Connection.IsConnected() && !Connection.CanModify() && !string.IsNullOrWhiteSpace(TrackerPassword))
                                    await Connection.SetPassword(TrackerPassword);
                            }
                            else
                            {
                                if (Connection.IsConnected())
                                    await Connection.Close();

                                Connection = await EurekaConnectionManager.JoinTracker(TrackerCode, TrackerPassword);
                            }
                        });
                    }
                }
                Utils.SetTooltip("Joins a tracker with the specified ID and password");

                ImGui.EndTable();
            }

            ImGui.PopStyleVar();

            ImGui.Spacing();
            DrawTrackerTable();
        }

        public async Task CreateTracker(int zoneId, bool printMessage = false)
        {
            (string trackerId, string password) = await EurekaConnectionManager.CreateTracker(zoneId);

            if (string.IsNullOrWhiteSpace(trackerId) && string.IsNullOrWhiteSpace(password))
            {
                PluginLog.Error("TrackerId and Password not returned from API for some reason.");
                return;
            }

            if (Connection.IsConnected())
                await Connection.Close();

            TrackerCode = trackerId; TrackerPassword = password;
            Connection = await EurekaConnectionManager.JoinTracker(trackerId, password);

            if (printMessage)
                EurekaHelper.PrintMessage($"Successfully created a tracker: {Utils.CombineUrl(Constants.EurekaTrackerLink, trackerId)}");
        }

        public async Task ExportTracker(string oldTrackerId, bool printMessage = false)
        {
            (string trackerId, string password) = await EurekaConnectionManager.ExportTracker(oldTrackerId);

            if (string.IsNullOrWhiteSpace(trackerId) && string.IsNullOrWhiteSpace(password))
            {
                PluginLog.Error("TrackerId and Password not returned from API for some reason.");
                return;
            }

            if (Connection.IsConnected())
                await Connection.Close();

            TrackerCode = trackerId; TrackerPassword = password;
            Connection = await EurekaConnectionManager.JoinTracker(trackerId, password);

            if (printMessage)
                EurekaHelper.PrintMessage($"Successfully exported the previous tracker: {Utils.CombineUrl(Constants.EurekaTrackerLink, trackerId)}");
        }

        public void DrawTrackerTable()
        {
            ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            ImGui.BeginChild("EurekaTracker", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true);
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();

            if (Connection.IsConnected())
            {
                var numColumns = 6;
                if (ImGui.BeginTable("TrackerTable", numColumns, ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.BordersV | ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.Sortable | ImGuiTableFlags.SortTristate))
                {
                    var levelTableColumnFlags = ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoSort;
                    if (!EurekaHelper.Config.ShowLevelInTrackerTable)
                        levelTableColumnFlags |= ImGuiTableColumnFlags.Disabled; 

                    ImGui.TableSetupColumn("Lv", levelTableColumnFlags);
                    ImGui.TableSetupColumn("NM", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoSort);
                    ImGui.TableSetupColumn("Spawned By", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoSort);
                    ImGui.TableSetupColumn("Popped At", ImGuiTableColumnFlags.NoSort);
                    ImGui.TableSetupColumn("Respawn In");
                    ImGui.TableSetupColumn("Reset All", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.NoSort);
                    ImGui.TableSetupScrollFreeze(0, 1);

                    ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
                    for (int column = 0; column < numColumns; column++)
                    {
                        ImGui.TableSetColumnIndex(column);
                        string columnName = ImGui.TableGetColumnName(column);
                        if (columnName != "Reset All")
                        {
                            ImGui.TableHeader(columnName);
                            continue;
                        }

                        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0.0f, 0.0f));
                        if (Connection.CanModify())
                        {
                            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.89f, 0.5f, 0.5f, 1.0f));
                            if (ImGui.Button("Reset All", new Vector2(ImGui.GetContentRegionAvail().X, 0.0f)))
                                ImGui.OpenPopup("Confirm");
                            ImGui.PopStyleColor();
                        }
                        else
                        {
                            ImGui.BeginDisabled();
                            ImGui.Button("Reset All", new Vector2(ImGui.GetContentRegionAvail().X, 0.0f));
                            ImGui.EndDisabled();
                        }

                        ImGui.PopStyleVar();
                    }

                    ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                    ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
                    if (ImGui.BeginPopup("Confirm"))
                    {
                        if (ImGui.SmallButton("Confirm?"))
                        {
                            _ = Task.Run(async () => { await Connection.ResetAll(); });
                            ImGui.CloseCurrentPopup();
                        }
                        ImGui.EndPopup();
                    }
                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor();

                    DrawTracker();

                    ImGui.EndTable();
                }
            }
            else
            {
                if (Connection.IsInvalid())
                    Utils.CenterText("Invalid Tracker");
                else
                    Utils.CenterText("Not connected to a tracker");
            }

            ImGui.EndChild();
        }

        static readonly Vector4 RedColor = new(0.89f, 0.5f, 0.5f, 1f);
        static readonly Vector4 BlueColor = new(0.26f, 0.44f, 0.64f, 1f);

        static readonly Vector4 GreenColorText = new(0.33f, 0.76f, 0.67f, 1f);
        static readonly Vector4 RedColorText = new(0.82f, 0.49f, 0.49f, 1f);
        static readonly Vector4 OrangeColorText = new(0.9f, 0.52f, 0f, 1f);

        private string TimeAgoHours = "0";
        private string TimeAgoMinutes = "0";
        private bool IsEditing = false;
        public void DrawTracker()
        {
            var zoneFates = Connection.GetTracker()?.GetFates().Where(x => x.IncludeInTracker).ToList();
            if (zoneFates is null)
                return;

            var minRowHeight = ImGui.GetContentRegionAvail().Y / zoneFates.Count;
            var spacing = ImGui.GetStyle().ItemInnerSpacing.X;

            var sortSpecs = ImGui.TableGetSortSpecs();
            if (sortSpecs.SpecsDirty)
            {
                var specsCount = sortSpecs.SpecsCount;
                if (specsCount > 0)
                {
                    switch (sortSpecs.Specs.SortDirection)
                    {
                        case ImGuiSortDirection.Ascending:
                            zoneFates = zoneFates.OrderBy(x => x.IsPopped())
                                .ThenBy(x => x.SpawnByRequiredWeather != EurekaWeather.None && x.SpawnByRequiredWeather != Connection.GetTracker().GetCurrentWeatherInfo().Weather)
                                .ThenBy(x => x.SpawnRequiredWeather != EurekaWeather.None && x.SpawnRequiredWeather != Connection.GetTracker().GetCurrentWeatherInfo().Weather)
                                .ThenBy(x => x.SpawnByRequiredNight && EorzeaTime.Now.EorzeaDateTime.Hour >= 6 && EorzeaTime.Now.EorzeaDateTime.Hour < 19)
                                .ToList();
                            break;

                        case ImGuiSortDirection.Descending:
                            zoneFates = zoneFates.OrderByDescending(x => x.IsPopped())
                                .ThenByDescending(x => x.SpawnByRequiredWeather != EurekaWeather.None && x.SpawnByRequiredWeather != Connection.GetTracker().GetCurrentWeatherInfo().Weather)
                                .ThenByDescending(x => x.SpawnRequiredWeather != EurekaWeather.None && x.SpawnRequiredWeather != Connection.GetTracker().GetCurrentWeatherInfo().Weather)
                                .ThenByDescending(x => x.SpawnByRequiredNight && EorzeaTime.Now.EorzeaDateTime.Hour >= 6 && EorzeaTime.Now.EorzeaDateTime.Hour < 19)
                                .ToList();
                            break;
                    }
                }
            }

            foreach (var fate in zoneFates)
            {
                ImGui.TableNextRow(ImGuiTableRowFlags.None, minRowHeight);
                
                // Fate Level
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(fate.FateLevel.ToString());

                // NM Boss Name
                ImGui.TableNextColumn();
                ImGui.Text(fate.BossName);
                if (ImGui.IsItemHovered())
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                    ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));

                    ImGui.BeginTooltip();
                    ImGui.Text($"FATE Name: {fate.FateName}");
                    ImGui.Text($"FATE Level: {fate.FateLevel}");
                    ImGui.Text($"Element:"); ImGui.SameLine(0.0f, spacing);
                    ImGui.TextColored(new Vector4(0.68f, 0.88f, 0.12f, 1.0f), fate.BossElement.ToFriendlyString());
                    if (fate.SpawnRequiredWeather != EurekaWeather.None)
                    {
                        ImGui.Text("Weather Required:");
                        ImGui.SameLine(0.0f, spacing);
                        ImGui.TextColored(new Vector4(1.0f, 0.4f, 1.0f, 1.0f), fate.SpawnRequiredWeather.ToFriendlyString());
                    }
                    ImGui.EndTooltip();

                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor();
                }
                if (ImGui.IsItemClicked())
                    Utils.SetFlagMarker(fate, openMap: true);

                // Spawned By
                ImGui.TableNextColumn();
                ImGui.Text(fate.SpawnedBy);
                if (ImGui.IsItemHovered())
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                    ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));

                    ImGui.BeginTooltip();
                    ImGui.Text($"Element:"); ImGui.SameLine(0.0f, spacing);
                    ImGui.TextColored(new Vector4(0.68f, 0.88f, 0.12f, 1.0f), fate.SpawnByElement.ToFriendlyString());

                    if (fate.SpawnByRequiredNight)
                        ImGui.Text("Night Required");

                    if (fate.SpawnByRequiredWeather != EurekaWeather.None)
                    {
                        ImGui.Text("Weather Required:");
                        ImGui.SameLine(0.0f, spacing);
                        ImGui.TextColored(new Vector4(1.0f, 0.4f, 1.0f, 1.0f), fate.SpawnByRequiredWeather.ToFriendlyString());
                    }

                    ImGui.EndTooltip();

                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor();
                }
                if (ImGui.IsItemClicked())
                    Utils.SetFlagMarker(fate.TerritoryId, fate.MapId, new Vector2(fate.SpawnByPosition.X, fate.SpawnByPosition.Y), openMap: true, drawCircle: true);

                // Popped At
                ImGui.TableNextColumn();
                if (fate.IsPopped())
                {
                    Utils.RightAlignTextInColumn(fate.GetPoppedTime().ToString("HH:mm"), RedColorText);
                    if (Connection.CanModify())
                    {
                        if (ImGui.IsItemClicked())
                        {
                            IsEditing = false;
                            ImGui.OpenPopup($"EditPopTime##{fate.TrackerId}");
                        }
                    }

                    ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                    ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
                    Utils.SetTooltip($"Popped on {fate.GetPoppedTime()} local time");
                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor();

                    ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                    ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
                    if (ImGui.BeginPopup($"EditPopTime##{fate.TrackerId}"))
                    {
                        if (!IsEditing)
                        {
                            var timeDiff = DateTime.Now - fate.GetPoppedTime();
                            TimeAgoHours = timeDiff.Hours.ToString();
                            TimeAgoMinutes = timeDiff.Minutes.ToString();

                            IsEditing = true;
                        }

                        unsafe
                        {
                            ImGui.Text("- TIME AGO -");
                            ImGui.Text($"NM: {fate.BossName}");

                            var width = ImGui.CalcTextSize("TIME").X;
                            ImGui.SetNextItemWidth(width);
                            ImGui.InputText($"hr##{fate.TrackerId}", ref TimeAgoHours, 1, ImGuiInputTextFlags.CharsDecimal | ImGuiInputTextFlags.CallbackCharFilter, IntegerCheck);
                            ImGui.SameLine();
                            ImGui.SetNextItemWidth(width);
                            ImGui.InputText($"min##{fate.TrackerId}", ref TimeAgoMinutes, 2, ImGuiInputTextFlags.CharsDecimal | ImGuiInputTextFlags.CallbackCharFilter, IntegerCheck);
                        }

                        if (string.IsNullOrWhiteSpace(TimeAgoHours))
                            TimeAgoHours = "0";
                        if (string.IsNullOrWhiteSpace(TimeAgoMinutes))
                            TimeAgoMinutes = "0";

                        var ts = new TimeSpan(int.Parse(TimeAgoHours), int.Parse(TimeAgoMinutes), 0);
                        ImGui.Text($"{ts.Hours} {(ts.Hours > 1 ? "hours" : "hour")} {ts.Minutes} {(ts.Minutes > 1 ? "minutes" : "minute")} ago");
                        if (ImGui.Button($"Set##{fate.TrackerId}", new Vector2(ImGui.GetContentRegionAvail().X, 0)))
                        {
                            var editedPopTime = DateTime.Now - ts;
                            _ = Task.Run(async () =>
                            {
                                await Connection.SetPopTime((ushort)fate.TrackerId, new DateTimeOffset(editedPopTime).ToUnixTimeMilliseconds());
                            });
                            ImGui.CloseCurrentPopup();
                        }
                        ImGui.EndPopup();
                    }

                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor();
                }

                // Respawn In:
                // Are as follow:
                // 1. Check if is popped, display time remaining
                // 2. Else, check if spawn mob requires weather
                // 3. Else, check if spawn mob requires night
                // 4. Else, check if fate requires weather
                // 5. Else, it's ready to be spawned
                ImGui.TableNextColumn();
                List<(string Action, string Time)> respawnRequirements = new(); // Format = {weather/day} #in: #{time}
                if (fate.IsPopped())
                {
                    respawnRequirements.Add(("Respawn", $"{(int)fate.GetRespawnTimeleft().TotalMinutes}m {fate.GetRespawnTimeleft().Seconds}s"));
                }

                if (fate.SpawnByRequiredWeather != EurekaWeather.None && fate.SpawnByRequiredWeather != Connection.GetTracker().GetCurrentWeatherInfo().Weather)
                {
                    var (Weather, Time) = Connection.GetTracker().GetAllNextWeatherTime().Single(x => x.Weather == fate.SpawnByRequiredWeather);
                    respawnRequirements.Add((Weather.ToFriendlyString(), Time.ToString(Time.Hours > 0 ? "hh'h 'mm'm 'ss's'" : "mm'm 'ss's'")));

                }
                else if (fate.SpawnRequiredWeather != EurekaWeather.None && fate.SpawnRequiredWeather != Connection.GetTracker().GetCurrentWeatherInfo().Weather)
                {
                    var (Weather, Time) = Connection.GetTracker().GetAllNextWeatherTime().Single(x => x.Weather == fate.SpawnRequiredWeather);
                    respawnRequirements.Add((Weather.ToFriendlyString(), Time.ToString(Time.Hours > 0 ? "hh'h 'mm'm 'ss's'" : "mm'm 'ss's'")));
                }

                if (fate.SpawnByRequiredNight && EorzeaTime.Now.EorzeaDateTime.Hour >= 6 && EorzeaTime.Now.EorzeaDateTime.Hour < 19)
                    respawnRequirements.Add(("Night", $"{EorzeaTime.Now.TimeUntilNight():mm'm 'ss's'}"));

                if (respawnRequirements.Count == 0)
                {
                    Utils.RightAlignTextInColumn("Ready", GreenColorText);
                }
                else
                {
                    Vector4 colorText;
                    if (respawnRequirements[0].Action == "Respawn")
                        colorText = RedColorText;
                    else
                        colorText = OrangeColorText;

                    Utils.RightAlignTextInColumn(respawnRequirements[0].Time, colorText);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                        ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));

                        ImGui.BeginTooltip();
                        foreach (var (Action, Time) in respawnRequirements)
                        {
                            if (Action == "Respawn")
                            {
                                ImGui.Text($"{Action} in: {Time}");
                            }
                            else if (Action == "Night")
                            {
                                ImGui.Text($"{Action} in: {Time}");
                            }
                            else
                            {
                                ImGui.TextColored(new Vector4(1.0f, 0.4f, 1.0f, 1.0f), Action); ImGui.SameLine(0.0f, ImGui.GetStyle().ItemInnerSpacing.X);
                                ImGui.Text($"in: {Time}");
                            }
                        }
                        ImGui.EndTooltip();

                        ImGui.PopStyleVar();
                        ImGui.PopStyleColor();
                    }
                }

                // Reset All
                ImGui.TableNextColumn();
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0.0f, 0.0f));
                if (fate.IsPopped())
                {
                    if (Connection.CanModify())
                    {
                        ImGui.PushStyleColor(ImGuiCol.Button, RedColor);
                        if (ImGui.Button($"RESET##{fate.TrackerId}", new Vector2(ImGui.GetColumnWidth(), 0.0f)))
                        {
                            _ = Task.Run(async () =>
                            {
                                await Connection.Reset((ushort)fate.TrackerId);
                            });
                        }
                        ImGui.PopStyleColor();
                    }
                    else
                    {
                        ImGui.BeginDisabled();
                        ImGui.Button("RESET", new Vector2(ImGui.GetColumnWidth(), 0.0f));
                        ImGui.EndDisabled();
                    }
                }
                else
                {
                    if (Connection.CanModify())
                    {
                        ImGui.PushStyleColor(ImGuiCol.Button, BlueColor);
                        if (ImGui.Button($"POP##{fate.TrackerId}", new Vector2(ImGui.GetColumnWidth(), 0.0f)))
                        {
                            _ = Task.Run(async () =>
                            {
                                await Connection.SetPopTime((ushort)fate.TrackerId, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                            });
                        }
                        ImGui.PopStyleColor();
                    }
                    else
                    {
                        ImGui.BeginDisabled();
                        ImGui.Button("POP", new Vector2(ImGui.GetColumnWidth(), 0.0f));
                        ImGui.EndDisabled();
                    }
                }
                ImGui.PopStyleVar();
            }
        }

        public static uint DefaultIcon = 60474;
        public static void ResetDefaultIcon() => DefaultIcon = 60474;

        public void DrawElementalTab()
        {
            ImGui.Columns(2);

            var save = false;

            save |= ImGui.Checkbox("Display Elemental", ref EurekaHelper.Config.DisplayElemental);
            Utils.SetTooltip("Displays in chat whenever an Elemental appears near the player");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Display Elemental Toast", ref EurekaHelper.Config.DisplayElementalToast);
            Utils.SetTooltip("Displays a toast whenever an Elemental appears near the player");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Crowdsource Locations", ref EurekaHelper.Config.ElementalCrowdsource);
            Utils.SetTooltip("Assist to crowdsource for Elemental locations");
            ImGui.NextColumn();

            ImGui.SetNextItemWidth(140f);
            var enumNames = Enum.GetNames<PayloadOptions>();
            var enumValues = Enum.GetValues<PayloadOptions>();
            var enumCurrent = Array.IndexOf(enumValues, EurekaHelper.Config.ElementalPayloadOptions);
            if (ImGui.Combo("Payload Options", ref enumCurrent, enumNames, enumNames.Length))
            {
                EurekaHelper.Config.ElementalPayloadOptions = enumValues[enumCurrent];
                save = true;
            }
            Utils.SetTooltip("Sets what the clickable payload does.\nThis also affects the Shout/Copy column in the table.\n" +
                "For example: Setting it to \'ShoutToChat\' will send the Elemental to current chat when you click the button.");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Auto Mark Elementals", ref EurekaHelper.Config.ElementalAutoMark);
            Utils.SetTooltip("Auto mark Elementals (only new Elementals) on map as you find them.\n" +
                "Due to some limitations, the map will always open when you find an Elemental with this configuration enabled.");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Always Clear Elementals", ref EurekaHelper.Config.ElementalAlwaysClear);
            Utils.SetTooltip("Always clear the Elemental list whenever you join a Eureka zone");
            ImGui.NextColumn();

            ImGui.Columns(1);

            if (save)
                EurekaHelper.Config.Save();

            ImGui.Separator();

            if (ImGui.Button("Add Known Elemental Map Markers"))
            {
                var territoryType = DalamudApi.ClientState.TerritoryType;
                if (Utils.IsPlayerInEurekaZone(territoryType))
                {
                    var knownLocations = ElementalManager.GetKnownLocations(territoryType);
                    foreach (var location in knownLocations)
                    {
                        // 61502 - X
                        // 63922 - MOOGLE
                        Utils.AddMapMarker(territoryType, location, 63922, true);
                    }
                }
                else
                {
                    EurekaHelper.PrintMessage("You must be in one of the Eureka zone to use this.");
                }
            }
            Utils.SetTooltip("Adds a marker to known Elemental positions on the current map and minimap.\n" +
                "Help contribute to the known locations by providing the developer the necessary information");
            ImGui.SameLine();

            if (ImGui.Button("Clear All Elementals"))
            {
                Plugin.ElementalManager.Elementals.Clear();
                ResetDefaultIcon();
            }
            ImGui.SameLine();

            if (ImGui.Button("Clear All Map Markers"))
            {
                Utils.ClearMapMarker();
                ResetDefaultIcon();
            }

            ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            ImGui.BeginChild("ElementalsChild", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true);
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();

            if (ImGui.BeginTable("ElementalsTable", 6, ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.BordersV | ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoSavedSettings))
            {
                ImGui.TableSetupColumn("Elemental");
                ImGui.TableSetupColumn("Location");
                ImGui.TableSetupColumn("Last Seen");
                ImGui.TableSetupColumn("S/C", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Mark", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Delete", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableHeadersRow();

                for (int i = Plugin.ElementalManager.Elementals.Count - 1; i >= 0; i--)
                {
                    var elemental = Plugin.ElementalManager.Elementals[i];

                    ImGui.TableNextColumn();
                    ImGui.Text(elemental.Name);

                    ImGui.TableNextColumn();
                    ImGui.Text($"X: {elemental.Position.X:0.0}, Y: {elemental.Position.Y:0.0}");
                    if (ImGui.IsItemClicked())
                        Utils.SetFlagMarker(elemental.TerritoryId, elemental.MapId, elemental.Position, openMap: true);

                    ImGui.TableNextColumn();
                    var dateTime = EorzeaTime.Zero.AddSeconds(elemental.LastSeen).ToLocalTime();
                    ImGui.Text(dateTime.ToString());

                    ImGui.TableNextColumn();
                    if (ImGui.Button($"{(EurekaHelper.Config.ElementalPayloadOptions == PayloadOptions.CopyToClipboard ? $"C##{elemental.ObjectId}" : $"S##{elemental.ObjectId}")}"))
                    {
                        Utils.SetFlagMarker(elemental.TerritoryId, elemental.MapId, elemental.Position);
                        switch (EurekaHelper.Config.ElementalPayloadOptions)
                        {
                            case PayloadOptions.CopyToClipboard:
                                Utils.CopyToClipboard($"{elemental.Name} <flag>");
                                break;

                            default:
                            case PayloadOptions.ShoutToChat:
                                Utils.SendMessage($"/sh {elemental.Name} <flag>");
                                break;
                        }
                    }

                    ImGui.TableNextColumn();
                    if (ImGuiComponents.IconButton($"Elemental{elemental.ObjectId}", FontAwesomeIcon.MapMarked))
                    {
                        Utils.AddMapMarker(elemental.TerritoryId, elemental.RawPosition, DefaultIcon, true);
                        DefaultIcon++;

                        if (DefaultIcon > 60476)
                            ResetDefaultIcon();
                    }

                    ImGui.TableNextColumn();
                    if (ImGuiComponents.IconButton($"Elemental{elemental.ObjectId}", FontAwesomeIcon.Trash))
                        Plugin.ElementalManager.Elementals.RemoveAt(i);
                }

                ImGui.EndTable();
            }

            ImGui.EndChild();
        }

        static string CustomMessages = string.Join("\n", EurekaHelper.Config.CustomMessages);
        public static void DrawSettingsTab()
        {
            ImGui.Columns(2, null, true);

            var save = false;

            save |= ImGui.Checkbox("Display NM Pop", ref EurekaHelper.Config.DisplayFatePop);
            Utils.SetTooltip("Displays the NM that popped in chat");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Enable NM pop sound", ref EurekaHelper.Config.PlayPopSound);
            Utils.SetTooltip("A sound que will be played whenever an NM pops.");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Display fate progress", ref EurekaHelper.Config.DisplayFateProgress);
            Utils.SetTooltip("Prints the NM progress in chat");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Enable bunny fates", ref EurekaHelper.Config.DisplayBunnyFates);
            Utils.SetTooltip("Enable display for bunny fates");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Display Toast", ref EurekaHelper.Config.DisplayToastPop);
            Utils.SetTooltip("Displays a toast whenever an NM pops");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Auto pop fate", ref EurekaHelper.Config.AutoPopFate);
            Utils.SetTooltip("Attempts to auto pop fate when connected to a tracker (if you have the password)");
            ImGui.NextColumn();

            ImGui.SetNextItemWidth(140f);
            var enumNames = Enum.GetNames<SoundEffect>();
            var enumValues = Enum.GetValues<SoundEffect>();
            var enumCurrent = Array.IndexOf(enumValues, EurekaHelper.Config.NMSoundEffect);
            if (ImGui.Combo("NM Sound Effect", ref enumCurrent, enumNames, enumNames.Length))
            {
                EurekaHelper.Config.NMSoundEffect = enumValues[enumCurrent];
                SoundManager.PlaySoundEffect(enumValues[enumCurrent]);
                save = true;
            }
            Utils.SetTooltip("Sound Effect to be played when an NM pops. Add 36 to the macro sound you want. Example: <se.5> is 5 + 36 = 41");
            ImGui.NextColumn();

            ImGui.SetNextItemWidth(140f);
            enumCurrent = Array.IndexOf(enumValues, EurekaHelper.Config.BunnySoundEffect);
            if (ImGui.Combo("Bunny Sound Effect", ref enumCurrent, enumNames, enumNames.Length))
            {
                EurekaHelper.Config.BunnySoundEffect = enumValues[enumCurrent];
                SoundManager.PlaySoundEffect(enumValues[enumCurrent]);
                save = true;
            }
            Utils.SetTooltip("Sound Effect to be played when bunny spawns. Add 36 to the macro sound you want. Example: <se.5> is 5 + 36 = 41");
            ImGui.NextColumn();

            ImGui.SetNextItemWidth(140f);
            enumNames = Enum.GetNames<PayloadOptions>();
            var poValues = Enum.GetValues<PayloadOptions>();
            enumCurrent = Array.IndexOf(poValues, EurekaHelper.Config.PayloadOptions);
            if (ImGui.Combo("Payload Options", ref enumCurrent, enumNames, enumNames.Length))
            {
                EurekaHelper.Config.PayloadOptions = poValues[enumCurrent];
                save = true;
            }
            Utils.SetTooltip("Sets what the clickable payload does.\n" +
                "For example: Setting it to \'ShoutToChat\' will shout the pop when you click the button in chat.");
            ImGui.NextColumn();

            ImGui.SetNextItemWidth(140f);
            enumNames = Enum.GetNames<XivChatType>();
            var chValues = Enum.GetValues<XivChatType>();
            enumCurrent = Array.IndexOf(chValues, EurekaHelper.Config.ChatChannel);
            if (ImGui.Combo("Chat Channel", ref enumCurrent, enumNames, enumNames.Length))
            {
                EurekaHelper.Config.ChatChannel = chValues[enumCurrent];
                save = true;
            }
            Utils.SetTooltip("Set the channel which the plugin messages will display. Default: Echo");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Randomize Map Coords", ref EurekaHelper.Config.RandomizeMapCoords);
            Utils.SetTooltip("Randomizes map coords to range of +- 0.5 (recommended to enable)");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Auto Create Tracker", ref EurekaHelper.Config.AutoCreateTracker);
            Utils.SetTooltip("Auto creates tracker when joining an instance and prints the tracker link to chat");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Auto Pop fate within range", ref EurekaHelper.Config.AutoPopFateWithinRange);
            Utils.SetTooltip("Requires \"Auto pop fate\" to be enabled.\n\n" +
                "NM fates has an estimated respawn time of 2 hours\n" +
                "This option will pop fates if it has a cooldown of less than 5 minutes instead of waiting for the normal 2 hour duration");
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Show Level On Tracker", ref EurekaHelper.Config.ShowLevelInTrackerTable);
            Utils.SetTooltip("Will show the level of a given NM in the tracker table.");
            ImGui.NextColumn();
            
            ImGui.Columns(1);
            if (ImGui.CollapsingHeader("Custom Messages"))
            {
                ImGui.TextWrapped("** HOW TO USE **" +
                    "\nType the messages you want in each line, to enter the next line press \"Enter\"\n");
                ImGui.TextWrapped("** AVAILABLE FORMATTINGS **");
                ImGui.BulletText("%%bossName%% - Replaced with fate boss name");
                ImGui.BulletText("%%bossShortName%% - Replaced with fate boss short name");
                ImGui.BulletText("%%fateName%% - Replaced with fate name");
                ImGui.BulletText("%%flag%% - Replaced with <flag>");
                ImGui.Spacing();
                ImGui.InputTextMultiline("###CustomShoutMessages", ref CustomMessages, 9999, new Vector2(-1, -1));
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    if (!string.IsNullOrWhiteSpace(CustomMessages))
                    {
                        EurekaHelper.Config.CustomMessages = CustomMessages.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        save = true;
                    }
                    else
                    {
                        CustomMessages = "/shout %bossName% POP. %flag%";
                        EurekaHelper.Config.CustomMessages = new() { CustomMessages };
                        save = true;
                    }
                }
            }

            if (save)
                EurekaHelper.Config.Save();
        }

        public static void DrawInstanceTab()
        {
            var save = false;

            ImGui.Columns(2);

            save |= ImGui.Checkbox("Display Server Id in chat", ref EurekaHelper.Config.DisplayServerId);
            ImGui.NextColumn();

            save |= ImGui.Checkbox("Display Server Id in \"server info\" bar", ref EurekaHelper.Config.DisplayServerIdInServerInfo);

            ImGui.Columns(1);

            ImGui.Separator();

            ImGui.TextColored(RedColorText, "** DISCLAIMER, READ THIS **");
            ImGui.TextWrapped("This option will display the current server ID of the instance in chat each time you instance into a Eureka zone. " +
                "This might help you identify unique instances. However, there are a few things you should note." +
                "\n\nFirst of all, this method is definitely not the best way to uniquely identify Eureka zones." +
                "\n\nSecondly, according to sources and self-testing, the server ID may get reused for the new instance after the old instance gets locked." +
                "\n\nExamples:\nIf you enter a Pagos zone with server ID (60) and you rejoin to another Pagos zone with server ID (61), it would have meant that you've just joined another instance." +
                "\nIf a zone in Pyros with server ID (59) gets locked, on very rare occasions, the new Pyros instance might get the same server ID (59) as well." +
                "\n\nThirdly, from what I know and have read (but have been unable to test), these server IDs are unique to people in the same world as you. " +
                "This means that another person in another world will get a different server ID than what you have." +
                "\n\nAfter reading all this information, I hope that you will use it only for your own good. And I will not be entertaining any feedback mentioning that the server ID is \"incorrect\".");

            if (save)
                EurekaHelper.Config.Save();
        }

        public static void DrawAboutTab()
        {
            ImGui.TextColored(new Vector4(0.0f, 1.0f, 0.0f, 1.0f), "About:");
            ImGui.Indent();
            ImGui.TextWrapped("Hi there!" +
                "\nThis is my first FFXIV plugin, alot of the ideas are shamelessly taken from other plugins." +
                "\n\nWelcome to Eureka Helper, a tool to help you on your Eureka Adventures. It offers a small variety of QoL changes and a built-in Eureka Tracker." +
                "\nFor those interested in money making NMs (e.g Cassie, Skoll), you can type /arisu (command name from ABBA discord) for their next weather time window!");
            ImGui.Unindent();
            ImGui.Dummy(new Vector2(0.0f, 10.0f));

            ImGui.TextColored(new Vector4(0.0f, 1.0f, 1.0f, 1.0f), "Information:");
            ImGui.Indent();
            ImGui.Text("GitHub:"); ImGui.SameLine(); Utils.TextURL("GitHub", "https://github.com/snooooowy/EurekaHelper", ImGui.GetColorU32(ImGuiCol.Text));
            ImGui.Text("Last commit:"); ImGui.SameLine(); ImGui.Text(Utils.GetGitSha());
            ImGui.Text("Version:"); ImGui.SameLine(); ImGui.Text(Utils.GetVersion());
            ImGui.Unindent();
            ImGui.Dummy(new Vector2(0.0f, 10.0f));

            ImGui.TextColored(new Vector4(1.0f, 0.7f, 0.06f, 1.0f), "Contact:");
            ImGui.Indent();
            ImGui.Text("Discord:"); ImGui.SameLine(); ImGui.Text("@snorux");
            ImGui.Text("Issues / Feedbacks:"); ImGui.SameLine(); Utils.TextURL("GitHub", "https://github.com/snooooowy/EurekaHelper/issues", ImGui.GetColorU32(ImGuiCol.Text));
            ImGui.Unindent();
            ImGui.Dummy(new Vector2(0.0f, 10.0f));

            ImGui.TextColored(ImGuiColors.ParsedPurple, "Commands");
            ImGui.Indent();
            ImGui.Text("/eurekahelper | /eh | /ehelper → Opens / Closes the configuration window");
            ImGui.Text("/etrackers → Attempts to get a tracker for the current instance in the same datacenter.");
            ImGui.Text("/erelic → Opens / Closes the Eureka Relic helper window");
            ImGui.Text("/ealarms → Opens / Closes the Eureka Alarms window");
            ImGui.Text("/arisu → Display next weather for Crab, Cassie & Skoll");
            ImGui.Unindent();
            ImGui.Dummy(new Vector2(0.0f, 10.0f));

            ImGui.TextColored(new Vector4(1.0f, 0.0f, 0.5f, 1.0f), "Credits:");
            ImGui.Indent();
            ImGui.Text("FFXIV Dev community");
            ImGui.Text("electr0sheep for EurekaTrackerAutoPopper");
            ImGui.Text("Bedo9041 for EurekaPlugin");
            ImGui.Text("KangasZ for EurekaHelper contributions");
        }

        public static EurekaConnectionManager GetConnection() => Connection;

        private unsafe int IntegerCheck(ImGuiInputTextCallbackData* data)
        {
            char c = Convert.ToChar(data->EventChar);

            if (c >= '0' && c <= '9')
                return 0;

            return 1;
        }
    }
}