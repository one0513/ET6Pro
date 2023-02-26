using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class TaskActionConfigCategory : ProtoObject, IMerge
    {
        public static TaskActionConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, TaskActionConfig> dict = new Dictionary<int, TaskActionConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<TaskActionConfig> list = new List<TaskActionConfig>();
		
        public TaskActionConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            TaskActionConfigCategory s = o as TaskActionConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                TaskActionConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public TaskActionConfig Get(int id)
        {
            this.dict.TryGetValue(id, out TaskActionConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (TaskActionConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, TaskActionConfig> GetAll()
        {
            return this.dict;
        }
        public List<TaskActionConfig> GetAllList()
        {
            return this.list;
        }
        public TaskActionConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class TaskActionConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>任务进度更新行为</summary>
		[ProtoMember(2)]
		public int TaskProgressType { get; set; }

	}
}
