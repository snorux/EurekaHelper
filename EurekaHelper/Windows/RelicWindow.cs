using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using EurekaHelper.XIV;
using EurekaHelper.XIV.Relic;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace EurekaHelper.Windows
{
    internal class RelicWindow : Window, IDisposable
    {
        private readonly EurekaHelper Plugin = null!;

        public RelicWindow(EurekaHelper plugin) : base("Eureka Helper - Relic")
        {
            Plugin = plugin;
            SizeConstraints = new WindowSizeConstraints { MinimumSize = new Vector2(415f, 500f), MaximumSize = new Vector2(float.MaxValue, float.MaxValue) };
        }

        public void Dispose() { }

        public override void Draw()
        {
            if (ImGui.BeginTabBar("ERelicTab"))
            {
                if (ImGui.BeginTabItem("Anemos"))
                {
                    DrawRelicTab(AnemosRelic.AnemosRelicStages);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Pagos"))
                {
                    DrawRelicTab(PagosRelic.AnemosRelicStages);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Pyros"))
                {
                    DrawRelicTab(PyrosRelic.PyrosRelicStages);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Hydatos"))
                {
                    DrawRelicTab(HydatosRelic.HydatosRelicStages);
                    ImGui.EndTabItem();
                }
            }
        }

        private void DrawRelicTab(Dictionary<string, EurekaRelic> stages)
        {
            ImGui.BeginChild("Relic", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), false);

            if (ImGui.BeginTabBar("RelicTabBar"))
            {
                foreach (var stage in stages)
                {
                    if (ImGui.BeginTabItem(stage.Key))
                    {
                        ImGui.TextColored(ImGuiColors.DalamudYellow, "Requirements:");
                        ImGui.SameLine();
                        ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                        ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
                        ImGuiComponents.HelpMarker("These are the requirements you need to complete/gather for each relic stage\n\n" +
                            "The first number shows the amount you have while the second number shows the amount you need\n" +
                            "However, for \"Elemental +1\" and \"Elemental +2\" armor relics, the numbers varies from 30-50 and 21-35 respectively\n" +
                            "To keep it simple, requirements will only show the maximum number of items needed\n\n" +
                            "If you keep the items in other inventories (ex. Saddlebags, Retainers), you need to open it at least once to update the count.");
                        ImGui.PopStyleVar();
                        ImGui.PopStyleColor();

                        foreach (var requirement in stage.Value.CompletionRequirements)
                        {
                            ImGui.Text(requirement.ItemName);
                            if (requirement.ItemId != 0)
                            {
                                if (ImGui.IsItemClicked())
                                {
                                    var itemLink = Utils.ItemLink(requirement.ItemId);
                                    EurekaHelper.PrintMessage(itemLink);
                                }
                            }

                            ImGui.SameLine();

                            var itemCount =  Plugin.InventoryManager.ScannedItems.TryGetValue(requirement.ItemId, out var inventoryCount) ? inventoryCount.Sum(x => x.Count) : 0;

                            if (requirement.ItemId != 0)
                            {
                                Utils.RightAlignTextInColumn($"{itemCount} / {requirement.ItemCount}", itemCount >= requirement.ItemCount ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed);

                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1f);
                                    ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));

                                    ImGui.BeginTooltip();

                                    if (Plugin.InventoryManager.ScannedItems.TryGetValue(requirement.ItemId, out var inventories))
                                    {
                                        var spacing = ImGui.GetStyle().ItemInnerSpacing.X;

                                        var characterInvCount = inventories.Where(x => x.InventoryType == InventoryType.Inventory1 
                                                                        || x.InventoryType == InventoryType.Inventory2 
                                                                        || x.InventoryType == InventoryType.Inventory3 
                                                                        || x.InventoryType == InventoryType.Inventory4)
                                                                        .Sum(x => x.Count);
                                        ImGui.Text("Inventories:"); ImGui.SameLine(0.0f, spacing);
                                        ImGui.TextColored(ImGuiColors.ParsedPink, $"{characterInvCount}");

                                        var saddleInvCount = inventories.Where(x => x.InventoryType == InventoryType.SaddleBag1
                                                                        || x.InventoryType == InventoryType.SaddleBag2
                                                                        || x.InventoryType == InventoryType.PremiumSaddleBag1
                                                                        || x.InventoryType == InventoryType.PremiumSaddleBag2)
                                                                        .Sum(x => x.Count);
                                        ImGui.Text("Saddlebags:"); ImGui.SameLine(0.0f, spacing);
                                        ImGui.TextColored(ImGuiColors.ParsedPink, $"{saddleInvCount}");

                                        var retainerInvCount = inventories.Where(x => x.InventoryType == InventoryType.RetainerPage1
                                                                        || x.InventoryType== InventoryType.RetainerPage2
                                                                        || x.InventoryType == InventoryType.RetainerPage3
                                                                        || x.InventoryType == InventoryType.RetainerPage4
                                                                        || x.InventoryType == InventoryType.RetainerPage5
                                                                        || x.InventoryType == InventoryType.RetainerPage6
                                                                        || x.InventoryType == InventoryType.RetainerPage7)
                                                                        .Sum(x => x.Count);
                                        ImGui.Text("Retainers:"); ImGui.SameLine(0.0f, spacing);
                                        ImGui.TextColored(ImGuiColors.ParsedPink, $"{retainerInvCount}");
                                    }
                                    else
                                    {
                                        ImGui.TextColored(ImGuiColors.DalamudRed, "Failed to get value for some reason, please contact author.");
                                    }

                                    ImGui.EndTooltip();

                                    ImGui.PopStyleVar();
                                    ImGui.PopStyleColor();
                                }
                            }
                            else
                            {
                                Utils.RightAlignTextInColumn($"{requirement.ItemCount}", ImGuiColors.DalamudOrange);
                            }
                        }

                        ImGui.Separator();

                        ImGui.PushStyleColor(ImGuiCol.Border, ImGui.GetColorU32(ImGuiCol.TabActive));
                        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
                        ImGui.BeginChild("ChildDisplay", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true);
                        ImGui.PopStyleColor();
                        ImGui.PopStyleVar();

                        if (ImGui.BeginTable("ItemsDisplayWindow", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.BordersV | ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.Sortable | ImGuiTableFlags.SortTristate))
                        {
                            ImGui.TableSetupScrollFreeze(0, 1);
                            ImGui.TableSetupColumn("Item", ImGuiTableColumnFlags.NoSort);
                            ImGui.TableSetupColumn("Job Category", ImGuiTableColumnFlags.WidthFixed);
                            ImGui.TableHeadersRow();

                            var currentStageRelics = stage.Value.RelicItems;

                            var sortSpecs = ImGui.TableGetSortSpecs();
                            if (sortSpecs.SpecsDirty)
                            {
                                var specsCount = sortSpecs.SpecsCount;
                                if (specsCount > 0)
                                {
                                    switch (sortSpecs.Specs.SortDirection)
                                    {
                                        case ImGuiSortDirection.Ascending:
                                            currentStageRelics = currentStageRelics.OrderBy(x => x.JobCategory)
                                                .ToList();
                                            break;

                                        case ImGuiSortDirection.Descending:
                                            currentStageRelics = currentStageRelics.OrderByDescending(x => x.JobCategory)
                                                .ToList();
                                            break;
                                    }
                                }
                            }

                            foreach (var relic in currentStageRelics)
                            {
                                ImGui.TableNextColumn();
                                ImGui.Text(relic.ItemName);
                                if (ImGui.IsItemClicked())
                                {
                                    var itemLink = Utils.ItemLink(relic.ItemId);
                                    EurekaHelper.PrintMessage(itemLink);
                                }

                                ImGui.TableNextColumn();
                                Utils.RightAlignTextInColumn(relic.JobCategory);
                            }

                            ImGui.EndTable();
                        }

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
