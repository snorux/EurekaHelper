using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using EurekaHelper.XIV;
using EurekaHelper.XIV.Relic;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace EurekaHelper.Windows
{
    internal class RelicWindow : Window, IDisposable
    {
        private readonly EurekaHelper Plugin = null!;

        public RelicWindow(EurekaHelper plugin) : base("Eureka Helper - Relic")
        {
            Plugin = plugin;
            SizeConstraints = new WindowSizeConstraints { MaximumSize = new Vector2(float.MaxValue, float.MaxValue) };
        }

        public void Dispose() { }

        public override void Draw()
        {
            if (ImGui.BeginTabBar("ERelicTab"))
            {
                if (ImGui.BeginTabItem("Anemos"))
                {
                    DrawRelicTab("Anemos", AnemosRelic.AnemosRelicStages);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Pagos"))
                {
                    DrawRelicTab("Pagos", PagosRelic.AnemosRelicStages);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Pyros"))
                {
                    DrawRelicTab("Pyros", PyrosRelic.PyrosRelicStages);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Hydatos"))
                {
                    DrawRelicTab("Hydatos", HydatosRelic.HydatosRelicStages);
                    ImGui.EndTabItem();
                }
            }
        }

        private void DrawRelicTab(string zone, Dictionary<string, EurekaRelic> stages)
        {
            ImGui.BeginChild($"##{zone}Relic", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), false);

            if (ImGui.BeginTabBar($"##{zone}RelicTabBar"))
            {
                foreach (var stage in stages)
                {
                    if (ImGui.BeginTabItem(stage.Key))
                    {
                        ImGui.TextColored(new Vector4(1.0f, 0.7f, 0.06f, 1.0f), "Requirements:");
                        ImGui.SameLine();
                        ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                        ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
                        ImGuiComponents.HelpMarker("This shows the amount of items you need for each relic\n" +
                            "However, for \"Elemental +1\" and \"Elemental +2\" armor relics, the numbers varies from 30-50 and 21-35 respectively\n" +
                            "To keep it simple, requirements will only show the maximum number of items needed\n\n" +
                            "If you keep the items in other inventories (ex. Saddlebags, Retainers), you need to open it at least once to update the count.");
                        ImGui.PopStyleVar();
                        ImGui.PopStyleColor();

                        foreach (var requirement in stage.Value.CompletionRequirements)
                        {
                            ImGui.Text($"{requirement.ItemName} - {requirement.ItemCount}");
                        }

                        ImGui.Separator();

                        ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
                        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
                        ImGui.BeginChild($"##ChildItem{stage.Key}", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true);
                        ImGui.PopStyleColor();
                        ImGui.PopStyleVar();

                        if (ImGui.BeginTable($"##Items{stage.Key}", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.BordersV | ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoSavedSettings))
                        {
                            ImGui.TableSetupScrollFreeze(0, 1);
                            ImGui.TableSetupColumn("Item");
                            ImGui.TableSetupColumn("Job Category", ImGuiTableColumnFlags.WidthFixed);
                            ImGui.TableHeadersRow();

                            foreach (var relic in stage.Value.RelicItems)
                            {
                                ImGui.TableNextColumn();
                                ImGui.Text(relic.ItemName);

                                ImGui.TableNextColumn();
                                Utils.RightAlignTextInColumn(relic.JobCategory);
                            }
                        }

                        ImGui.EndTable();
                        ImGui.EndChild();
                        ImGui.EndTabItem();
                    }
                }
            }

            ImGui.EndTabBar();
            ImGui.EndChild();
        }
    }
}
