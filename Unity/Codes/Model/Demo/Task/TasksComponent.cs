using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [ComponentOf]
    [ChildOf(typeof(TaskInfo))]
#if SERVER
    public class TasksComponent : Entity,IAwake,IDestroy,ITransfer,IUnitCache, IDeserialize
#else
    public class TasksComponent : Entity,IAwake,IDestroy
#endif
    {
#if SERVER
        [BsonIgnore]
#endif
        public SortedDictionary<int,TaskInfo> TaskInfoDict = new SortedDictionary<int, TaskInfo>();

#if !SERVER
        public List<TaskInfo> TaskInfoList = new List<TaskInfo>();
#endif
        
#if SERVER
        [BsonIgnore]
        public HashSet<int> CurrentTaskSet = new HashSet<int>();
#endif
        
#if SERVER
        [BsonIgnore]
        public M2C_UpdateTaskInfo M2CUpdateTaskInfo = new M2C_UpdateTaskInfo();
#endif
        
    }
}