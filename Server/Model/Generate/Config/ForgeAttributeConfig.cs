using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class ForgeAttributeConfigCategory : ProtoObject, IMerge
    {
        public static ForgeAttributeConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, ForgeAttributeConfig> dict = new Dictionary<int, ForgeAttributeConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<ForgeAttributeConfig> list = new List<ForgeAttributeConfig>();
		
        public ForgeAttributeConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            ForgeAttributeConfigCategory s = o as ForgeAttributeConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                ForgeAttributeConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public ForgeAttributeConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ForgeAttributeConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (ForgeAttributeConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, ForgeAttributeConfig> GetAll()
        {
            return this.dict;
        }
        public List<ForgeAttributeConfig> GetAllList()
        {
            return this.list;
        }
        public ForgeAttributeConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class ForgeAttributeConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>包含属性</summary>
		[ProtoMember(2)]
		public int[] Attributes { get; set; }
		/// <summary>属性范围</summary>
		[ProtoMember(3)]
		public int[] AttributeRage { get; set; }

	}
}
