using UnityEngine;
using TMPro;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class HeadHpViewComponent: Entity,IAwake,IDestroy
    {
        public GameObject HpBarGroup = null;
        public SpriteRenderer HpBar = null;
        public TextMeshPro HpText = null;

        public GameObject DidTip = null;
        public TextMeshPro RelifeTimeText = null;
    }
    
}