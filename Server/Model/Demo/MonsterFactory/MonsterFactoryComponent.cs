using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class MonsterFactoryComponent: Entity, IAwake, IDestroy
    {
        public Dictionary<long, List<long>> roomIdAndEnemyId = new Dictionary<long, List<long>>();
        public List<Vector3> randomPos = new List<Vector3>()
        {
            new Vector3(-2,0.1f,2),
            new Vector3(0,0.1f,2),
            new Vector3(2,0.1f,2),
            new Vector3(-2,0.1f,3.5f),
            new Vector3(0,0.1f,3.5f),
            new Vector3(2,0.1f,3.5f),
        };
      
    }
}