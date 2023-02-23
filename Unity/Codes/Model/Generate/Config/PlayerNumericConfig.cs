using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class PlayerNumericConfigCategory : ProtoObject, IMerge
    {
        public static PlayerNumericConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, PlayerNumericConfig> dict = new Dictionary<int, PlayerNumericConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<PlayerNumericConfig> list = new List<PlayerNumericConfig>();
		
        public PlayerNumericConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            PlayerNumericConfigCategory s = o as PlayerNumericConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                PlayerNumericConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public PlayerNumericConfig Get(int id)
        {
            this.dict.TryGetValue(id, out PlayerNumericConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (PlayerNumericConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, PlayerNumericConfig> GetAll()
        {
            return this.dict;
        }
        public List<PlayerNumericConfig> GetAllList()
        {
            return this.list;
        }
        public PlayerNumericConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class PlayerNumericConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>名字</summary>
		[ProtoMember(2)]
		public string Name { get; set; }
		/// <summary>是否用于展示</summary>
		[ProtoMember(4)]
		public int isNeedShow { get; set; }
		/// <summary>是否是百分比</summary>
		[ProtoMember(6)]
		public int isPrecent { get; set; }

	}
}
