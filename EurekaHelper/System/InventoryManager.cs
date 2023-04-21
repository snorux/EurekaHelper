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

        public Dictionary<uint, int> ScannedItems = new()
        {
            [21801] = 0, // Protean Crystal
            [21802] = 0, // Pazuzu's feather
            [21803] = 0, // Anemos Crystal

            [23309] = 0, // Frosted Protean Crystal
            [22976] = 0, // Pagos Crystal
            [22975] = 0, // Louhi's Ice

            [24124] = 0, // Pyros Crystal
            [24123] = 0, // Penthesilea's Flame

            [24807] = 0, // Hydatos Crystal
            [24806] = 0, // Crystalline Scale
            [24808] = 0, // Eureka Fragment
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
                    int totalCount = 0;
                    
                    foreach (var inventoryType in InventoriesToScan)
                    {
                        totalCount += inventoryManger->GetItemCountInContainer(item.Key, inventoryType);
                    }

                    ScannedItems[item.Key] = totalCount;
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Failed to scan for items.");
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
}
