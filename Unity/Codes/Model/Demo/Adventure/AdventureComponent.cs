using System;
using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class AdventureComponent : Entity,IAwake,IDestroy
    {
        public long BattleTimer = 0;
        
        public int Round = 0;
        
        public List<long> EnemyIdList = new List<long>();
        
        public List<long> AliveEnemyIdList = new List<long>();

        public SRandom Random = null;

        public bool isFighting = false;
    }
}