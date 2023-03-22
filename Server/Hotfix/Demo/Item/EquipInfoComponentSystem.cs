namespace ET
{
    public class EquipInfoComponentAwakeSystem : AwakeSystem<EquipInfoComponent>
    {
        public override void Awake(EquipInfoComponent self)
        {
            //self.GenerateEntries();
        }
    }
    
    public class EquipInfoComponentDestorySystem: DestroySystem<EquipInfoComponent>
    {
        public override void Destroy(EquipInfoComponent self)
        {
            self.IsInited = false;
            self.Score    = 0;

            foreach (var entry in self.EntryList)
            {
                entry?.Dispose();
            }
            self.EntryList.Clear();
        }
    }
    
    public class EquipInfoComponentDeserializeSystem : DeserializeSystem<EquipInfoComponent>
    {
        public override void Deserialize(EquipInfoComponent self)
        {
            foreach (var entity in self.Children.Values)
            {
                self.EntryList.Add(entity as AttributeEntry);
            }
        }
    }

    
    [FriendClass(typeof(Item))]
    [FriendClass(typeof(AttributeEntry))]
    [FriendClass(typeof(EquipInfoComponent))]
    public static class EquipInfoComponentSystem
    {
        public static void GenerateEntries(this EquipInfoComponent self)
        {
            if (self.IsInited)
            {
                return;
            }

            self.IsInited = true;
            self.CreateEntry();
        }


        public static void CreateOneEntry(this EquipInfoComponent self,int key,long value)
        {
            value += 1;
            foreach (var attr in self.EntryList)
            {
                if (attr.Key == key)
                {
                    attr.Value += RandomHelper.RandomNumber(0,(int)value);
                    return;
                }
            }
            int random =RandomHelper.RandomNumber(0,(int)value);
            if (random == 0)
                return;
            
            AttributeEntry attributeEntry = self.AddChild<AttributeEntry>();
            attributeEntry.Type           = EntryType.Common;
            attributeEntry.Key            = key;
            attributeEntry.Value          = random;
            self.EntryList.Add(attributeEntry);
        }
        
        public static void CreateEntry(this EquipInfoComponent self)
        {
            ItemConfig itemConfig = self.GetParent<Item>().Config;
            
            EntryRandomConfig entryRandomConfig = EntryRandomConfigCategory.Instance.Get(1);

            //创建普通词条
            int entryCount = RandomHelper.RandomNumber(entryRandomConfig.EntryRandMinCount + self.GetParent<Item>().Quality, entryRandomConfig.EntryRandMaxCount + self.GetParent<Item>().Quality);
            for (int i = 0; i < entryCount; i++)
            {
                EntryConfig entryConfig       = EntryConfigCategory.Instance.GetRandomEntryConfigByLevel((int)EntryType.Common,entryRandomConfig.EntryLevel);
                if (entryConfig == null)
                {
                    continue;
                }
                
                AttributeEntry attributeEntry = self.AddChild<AttributeEntry>();
                attributeEntry.Type           = EntryType.Common;
                attributeEntry.Key            = entryConfig.AttributeType;
                attributeEntry.Value          = RandomHelper.RandomNumber(entryConfig.AttributeMinValue, entryConfig.AttributeMaxValue + self.GetParent<Item>().Quality);
                self.EntryList.Add(attributeEntry);
                self.Score += entryConfig.EntryScore;
            }
            
            
            //创建特殊词条
            entryCount = RandomHelper.RandomNumber(entryRandomConfig.SpecialEntryRandMinCount, entryRandomConfig.SpecialEntryRandMaxCount );
            for (int i = 0; i < entryCount; i++)
            {
                EntryConfig entryConfig       = EntryConfigCategory.Instance.GetRandomEntryConfigByLevel((int)EntryType.Special,entryRandomConfig.SpecialEntryLevel);
                if (entryConfig == null)
                {
                    continue;
                }
                AttributeEntry attributeEntry = self.AddChild<AttributeEntry>();
                attributeEntry.Type           = EntryType.Special;
                attributeEntry.Key            = entryConfig.AttributeType;
                attributeEntry.Value          = RandomHelper.RandomNumber(entryConfig.AttributeMinValue, entryConfig.AttributeMaxValue);
                self.EntryList.Add(attributeEntry);
                self.Score += entryConfig.EntryScore;
            }
            
        }
        
        
        public static EquipInfoProto ToMessage(this EquipInfoComponent self)
        {
            EquipInfoProto equipInfoProto = new EquipInfoProto();
            equipInfoProto.Id = self.Id;
            equipInfoProto.Score = self.Score;
            for (int i = 0; i < self.EntryList.Count; i++)
            {
                equipInfoProto.AttributeEntryProtoList.Add(self.EntryList[i].ToMessage());
            }
            return equipInfoProto;
        }
    }
}