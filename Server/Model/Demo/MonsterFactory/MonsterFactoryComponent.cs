using System.Collections.Generic;
namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class MonsterFactoryComponent: Entity, IAwake, IDestroy
    {
        public Dictionary<long, List<long>> roomIdAndEnemyId = new Dictionary<long, List<long>>();
        //public List<long> EnemyIdList  = new List<long>();
    }
}