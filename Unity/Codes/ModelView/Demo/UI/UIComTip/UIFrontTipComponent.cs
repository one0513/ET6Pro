using UnityEngine;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class UIFrontTipComponent:Entity,IAwake,IDestroy
    {
        public static UIFrontTipComponent Instance;
        public Transform root; 
    }
}