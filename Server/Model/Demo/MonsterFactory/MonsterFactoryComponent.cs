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
            new Vector3(1,0.1f,3),
            new Vector3(1.5f,0.1f,1),
            new Vector3(2.5f,0.1f,-1),
            new Vector3(3,0.1f,4),
            new Vector3(2,0.1f,-1),
            new Vector3(-2,0.1f,-2),
            new Vector3(-3,0.1f,-1),
            new Vector3(1.5f,0.1f,0.5f),
            new Vector3(3,0.1f,-3),
            new Vector3(-3,0.1f,-4),
            new Vector3(2,0.1f,4),
            new Vector3(2,0.1f,-4),
        };
      
    }
}