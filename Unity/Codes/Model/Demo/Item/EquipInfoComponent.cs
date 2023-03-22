using System;
using System.Collections.Generic;
#if SERVER
using MongoDB.Bson.Serialization.Attributes;
#endif


namespace ET
{
    [ComponentOf(typeof(Item))]
    [ChildOf(typeof(AttributeEntry))]
#if SERVER
    public class EquipInfoComponent : Entity,IAwake,IDestroy,ISerializeToEntity,IDeserialize
#else
    public class EquipInfoComponent : Entity,IAwake,IDestroy
#endif
    {
        public bool IsInited = false;
        
        /// <summary>
        /// 装备评分
        /// </summary>
        public int Score = 0;
        
        /// <summary>
        /// 装备词条列表
        /// </summary>
#if SERVER
        [BsonIgnore]
#endif
        public List<AttributeEntry> EntryList = new List<AttributeEntry>();
    }
}