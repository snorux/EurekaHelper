using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EurekaHelper.System
{
    public class InventoryManager : IDisposable
    {
        private readonly CancellationTokenSource CancellationTokenSource = new();

        private readonly InventoryType[] InventoriesToScan = new[]
        {
            InventoryType.Inventory1,
            InventoryType.Inventory2,
            InventoryType.Inventory3,
            InventoryType.Inventory4,
            InventoryType.SaddleBag1,
            InventoryType.SaddleBag2,
            InventoryType.PremiumSaddleBag1,
            InventoryType.PremiumSaddleBag2,
            InventoryType.RetainerPage1,
            InventoryType.RetainerPage2,
            InventoryType.RetainerPage3,
            InventoryType.RetainerPage4,
            InventoryType.RetainerPage5,
            InventoryType.RetainerPage6,
            InventoryType.RetainerPage7,
        };

        public Dictionary<uint, List<InventoryCount>> ScannedItems = new()
        {
            [21801] = new(), // Protean Crystal
            [21802] = new(), // Pazuzu's feather
            [21803] = new(), // Anemos Crystal

            [23309] = new(), // Frosted Protean Crystal
            [22976] = new(), // Pagos Crystal
            [22975] = new(), // Louhi's Ice

            [24124] = new(), // Pyros Crystal
            [24123] = new(), // Penthesilea's Flame

            [24807] = new(), // Hydatos Crystal
            [24806] = new(), // Crystalline Scale
            [24808] = new(), // Eureka Fragment
        };

        public InventoryManager()
        {
            DalamudApi.Framework.RunOnTick(ScanInventories, TimeSpan.FromMilliseconds(500), cancellationToken: CancellationTokenSource.Token);
        }

        private unsafe void ScanInventories()
        {
            try
            {
                var inventoryManger = FFXIVClientStructs.FFXIV.Client.Game.InventoryManager.Instance();

                foreach (var item in ScannedItems.ToList())
                {
                    foreach (var inventoryType in InventoriesToScan)
                    {
                        int totalCount = 0;

                        var inventory = inventoryManger->GetInventoryContainer(inventoryType);
                        if (inventory == null)
                            continue;

                        if (inventory->Loaded == 0)
                            continue;

                        totalCount += inventoryManger->GetItemCountInContainer(item.Key, inventoryType);

                        if (ScannedItems[item.Key].Any(x => x.InventoryType == inventoryType))
                            ScannedItems[item.Key].Single(x => x.InventoryType == inventoryType).Count = totalCount;
                        else
                            ScannedItems[item.Key].Add(new(inventoryType, totalCount));
                    }
                }
            }
            catch (Exception ex)
            {
                DalamudApi.Log.Error(ex, "Failed to scan for items.");
            }
            finally
            {
                DalamudApi.Framework.RunOnTick(ScanInventories, TimeSpan.FromMilliseconds(500), cancellationToken: CancellationTokenSource.Token);
            }
        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel();
        }
    }

    public class InventoryCount
    {
        public InventoryType InventoryType { get; set; }
        public int Count { get; set; }

        public InventoryCount(InventoryType inventoryType, int count)
        {
            InventoryType = inventoryType;
            Count = count;
        }
    }
}
