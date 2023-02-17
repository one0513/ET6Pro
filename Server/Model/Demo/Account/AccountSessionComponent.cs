using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class AccountSessionComponent : Entity,IAwake,IDestroy
    {
        public Dictionary<long, long> accountSessionDict = new Dictionary<long, long>();
        
        

    }
}