using UnityEngine;

namespace ET
{
    
    public class EquipInfoComponentDestorySystem: DestroySystem<EquipInfoComponent>
    {
        public override void Destroy(EquipInfoComponent self)
        {
            self.IsInited  = false;
            self.Score     = 0;
            for (int i = 0; i < self.EntryList.Count; i++)
            {
                self.EntryList[i]?.Dispose();
            }
            self.EntryList.Clear();
        }
    }

    
    [FriendClass(typeof(AttributeEntry))]
    [FriendClass(typeof(EquipInfoComponent))]
    public static class EquipInfoComponentSystem
    {
        public static void FromMessage(this EquipInfoComponent self,EquipInfoProto equipInfoProto)
        {
            self.Score = equipInfoProto.Score;

            for (int i = 0; i < self.EntryList.Count; i++)
            {
                self.EntryList[i]?.Dispose();
            }
            self.EntryList.Clear();
            
            for (int i = 0; i < equipInfoProto.AttributeEntryProtoList.Count; i++)
            {
                AttributeEntry attributeEntry = self.AddChild<AttributeEntry>();
                attributeEntry.FromMessage(equipInfoProto.AttributeEntryProtoList[i]);
                self.EntryList.Add(attributeEntry);
            }

            self.IsInited = true;
        }
    }

}