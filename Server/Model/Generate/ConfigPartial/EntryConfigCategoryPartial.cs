using System.Collections.Generic;

namespace ET
{
    public partial class EntryConfigCategory
    {
        private Dictionary<int, MultiMap<int, EntryConfig>> EntryConfigsDict = new Dictionary<int, MultiMap<int, EntryConfig>>();



        public override void AfterEndInit()
        {
            base.AfterEndInit();
            
            foreach (var config in this.dict.Values)
            {
                if (!this.EntryConfigsDict.ContainsKey(config.EntryType))
                {
                    this.EntryConfigsDict.Add(config.EntryType,new MultiMap<int, EntryConfig>());
                }
                this.EntryConfigsDict[config.EntryType].Add(config.EntryLevel,config);
            }
        }

        public EntryConfig GetRandomEntryConfigByLevel(int entryType, int level)
        {
            if (!this.EntryConfigsDict.ContainsKey(entryType))
            {
                return null;
            }
            
            MultiMap<int, EntryConfig> entryConfigsMap = this.EntryConfigsDict[entryType];
            if (!entryConfigsMap.ContainsKey(level))
            {
                return null;
            }
            
            var configList = entryConfigsMap[level];
            int index      = RandomHelper.RandomNumber(0, configList.Count);
            return configList[index];
        }
    }
}