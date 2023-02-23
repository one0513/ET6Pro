using System.Collections.Generic;

namespace ET
{

    public interface IUnitCache
    {
        
    }
    
    
    [ChildOf(typeof(UnitCacheComponent))]
    public class UnitCache : Entity,IAwake,IDestroy
    {
        public string key;

        public Dictionary<long, Entity> CacheCompoenntsDictionary = new Dictionary<long, Entity>();
    }
}