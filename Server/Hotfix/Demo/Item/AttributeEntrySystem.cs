namespace ET
{
    
    public class AttributeEntryDestorySystem: DestroySystem<AttributeEntry>
    {
        public override void Destroy(AttributeEntry self)
        {
            self.Key   = 0;
            self.Value = 0;
            self.Type  = EntryType.Common;
        }
    }

    
    
    [FriendClass(typeof(AttributeEntry))]
    public static class AttributeEntrySystem
    {
        public static AttributeEntryProto ToMessage(this AttributeEntry self)
        {
            AttributeEntryProto attributeEntryProto = new AttributeEntryProto();
            attributeEntryProto.Id    = self.Id;
            attributeEntryProto.Key   = self.Key;
            attributeEntryProto.Value = self.Value;
            attributeEntryProto.EntryType = (int)self.Type;
            return attributeEntryProto;
        }
    }
}