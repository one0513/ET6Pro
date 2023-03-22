using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class EntryConfigCategory : ProtoObject, IMerge
    {
        public static EntryConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, EntryConfig> dict = new Dictionary<int, EntryConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<EntryConfig> list = new List<EntryConfig>();
		
        public EntryConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            EntryConfigCategory s = o as EntryConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                EntryConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public EntryConfig Get(int id)
        {
            this.dict.TryGetValue(id, out EntryConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (EntryConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, EntryConfig> GetAll()
        {
            return this.dict;
        }
        public List<EntryConfig> GetAllList()
        {
            return this.list;
        }
        public EntryConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class EntryConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>词条类型</summary>
		[ProtoMember(2)]
		public int EntryType { get; set; }
		/// <summary>词条等级</summary>
		[ProtoMember(3)]
		public int EntryLevel { get; set; }
		/// <summary>词条评分</summary>
		[ProtoMember(4)]
		public int EntryScore { get; set; }
		/// <summary>属性类型</summary>
		[ProtoMember(5)]
		public int AttributeType { get; set; }
		/// <summary>属性值最小范围</summary>
		[ProtoMember(6)]
		public int AttributeMinValue { get; set; }
		/// <summary>属性值最大范围</summary>
		[ProtoMember(7)]
		public int AttributeMaxValue { get; set; }

	}
}
