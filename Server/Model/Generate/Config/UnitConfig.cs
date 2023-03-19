using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class UnitConfigCategory : ProtoObject, IMerge
    {
        public static UnitConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, UnitConfig> dict = new Dictionary<int, UnitConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<UnitConfig> list = new List<UnitConfig>();
		
        public UnitConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            UnitConfigCategory s = o as UnitConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                UnitConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public UnitConfig Get(int id)
        {
            this.dict.TryGetValue(id, out UnitConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (UnitConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, UnitConfig> GetAll()
        {
            return this.dict;
        }
        public List<UnitConfig> GetAllList()
        {
            return this.list;
        }
        public UnitConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class UnitConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>Type</summary>
		[ProtoMember(2)]
		public int Type { get; set; }
		/// <summary>名字</summary>
		[ProtoMember(3)]
		public string Name { get; set; }
		/// <summary>描述</summary>
		[ProtoMember(4)]
		public string Desc { get; set; }
		/// <summary>最大生命值</summary>
		[ProtoMember(5)]
		public int MaxHP { get; set; }
		/// <summary>攻击</summary>
		[ProtoMember(6)]
		public int Atk { get; set; }
		/// <summary>防御</summary>
		[ProtoMember(7)]
		public int Def { get; set; }
		/// <summary>伤害</summary>
		[ProtoMember(8)]
		public int Dmg { get; set; }
		/// <summary>攻击速度</summary>
		[ProtoMember(9)]
		public int ATKSpeed { get; set; }
		/// <summary>攻击范围</summary>
		[ProtoMember(10)]
		public int ATKRage { get; set; }
		/// <summary>移动速度</summary>
		[ProtoMember(11)]
		public int Speed { get; set; }
		/// <summary>等级</summary>
		[ProtoMember(12)]
		public int Lv { get; set; }
		/// <summary>掉落经验</summary>
		[ProtoMember(14)]
		public int Drop { get; set; }

	}
}
