using System.Collections.Generic;

namespace EurekaHelper.XIV
{
    public class EurekaRelic
    {
        public List<RelicItem> RelicItems { get; set; }
        public List<CompletionRequirement> CompletionRequirements { get; set; }

        public EurekaRelic(List<RelicItem> relicItems, List<CompletionRequirement> completionRequirements) 
        {
            RelicItems = relicItems;
            CompletionRequirements = completionRequirements;
        }
    }

    public class RelicItem
    {
        public uint ItemId { get; set; }
        public string ItemName { get; set; }
        public string JobCategory { get; set; }

        public RelicItem(uint itemId)
        { 
            ItemId = itemId;
            ItemName = Utils.GetItemName(itemId);
            JobCategory = Utils.GetJobCategory(itemId);
        }
    }

    public class CompletionRequirement
    {
        public uint ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemCount { get; set; }

        public CompletionRequirement(uint itemId, int itemCount, string customItemName = "")
        {
            ItemId = itemId;
            ItemName = itemId != 0 ? Utils.GetItemName(itemId) : customItemName;
            ItemCount = itemCount;
        }
    }
}
