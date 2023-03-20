using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace ET
{
    [FriendClass(typeof(GameObjectComponent))]
    public class HeadHpViewComponentAwakeSystem : AwakeSystem<HeadHpViewComponent>
    {
        public override void Awake(HeadHpViewComponent self)
        {
            GameObject gameObject = self.GetParent<Unit>().GetComponent<GameObjectComponent>().GameObject;
            self.HpBarGroup = gameObject.GetComponent<ReferenceCollector>().GetObject("HpBarGroup") as GameObject;
            self.HpText     = (gameObject.GetComponent<ReferenceCollector>().GetObject("HpText") as GameObject).GetComponent<TextMeshPro>();
            self.HpBar      = (gameObject.GetComponent<ReferenceCollector>().GetObject("HpBar") as GameObject).GetComponent<SpriteRenderer>();

            if (((Unit)self.Parent).Type == UnitType.Player)
            {
                self.DidTip = gameObject.GetComponent<ReferenceCollector>().GetObject("DidTip") as GameObject;
                self.RelifeTimeText     = (gameObject.GetComponent<ReferenceCollector>().GetObject("RelifeTimeText") as GameObject).GetComponent<TextMeshPro>();
            }
        }
    }

    [FriendClass(typeof(HeadHpViewComponent))]
    public static class HeadHpViewComponentSystem
    {
        public static void SetVisible(this HeadHpViewComponent self, bool isVisible)
        {
            self.HpBarGroup?.SetActive(isVisible);
        }

        public static async ETTask SetHp(this HeadHpViewComponent self)
        {
            
            NumericComponent numericComponent = self.GetParent<Unit>().GetComponent<NumericComponent>();

            int MaxHp = numericComponent.GetAsInt(NumericType.MaxHp);
            int Hp    = numericComponent.GetAsInt(NumericType.Hp);

            Hp = Hp < 0? 0 : Hp;
            if (Hp != MaxHp)
            {
                await TimerComponent.Instance.WaitAsync(1000);
            }
            self.HpText.text = $"{Hp} / {MaxHp}";
            self.HpBar.size = new Vector2((float)Hp / MaxHp, self.HpBar.size.y);
            await ETTask.CompletedTask;
        }
        
        public static void SetDieState(this HeadHpViewComponent self,int time)
        {
            if (time == 0)
            {
                int MaxHp = self.Parent.GetComponent<NumericComponent>().GetAsInt(NumericType.MaxHp);
                self.Parent.GetComponent<NumericComponent>().SetNoEvent(NumericType.Hp,MaxHp);
            }
            self.DidTip?.SetActive(time >0);
            self.RelifeTimeText.text = $"{time}";
        }
        
    }
}