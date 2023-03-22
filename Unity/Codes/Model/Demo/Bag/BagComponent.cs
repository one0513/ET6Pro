using System.Collections.Generic;
#if SERVER
using MongoDB.Bson.Serialization.Attributes;
#endif

namespace ET
{
    [ComponentOf]
    [ChildOf(typeof(Item))]
#if SERVER
    public class BagComponent : Entity,IAwake,IDestroy,IDeserialize,ITransfer,IUnitCache
#else
    public class BagComponent : Entity,IAwake,IDestroy
#endif
    {
#if SERVER
        [BsonIgnore]
#endif
        public Dictionary<long, Item> ItemsDict = new Dictionary<long, Item>(); 
        
#if SERVER
        [BsonIgnore]
#endif
        //只存道具 用来记录道具叠加 装备不可叠加
        public Dictionary<int, long> CfgAndItemIDDict = new Dictionary<int, long>(); 

#if SERVER
        [BsonIgnore]
#endif
        public MultiMap<int, Item> ItemsMap = new MultiMap<int, Item>();

#if SERVER
        [BsonIgnore]
        public M2C_ItemUpdateOpInfo message = new M2C_ItemUpdateOpInfo() {ContainerType = (int)ItemContainerType.Bag};
#endif
  

       
    }
}