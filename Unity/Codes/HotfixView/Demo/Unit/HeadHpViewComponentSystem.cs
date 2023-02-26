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
        }
    }

    [FriendClass(typeof(HeadHpViewComponent))]
    public static class HeadHpViewComponentSystem
    {
        public static void SetVisible(this HeadHpViewComponent self, bool isVisible)
        {
            self.HpBarGroup?.SetActive(isVisible);
        }

        public static void SetHp(this HeadHpViewComponent self)
        {
            NumericComponent numericComponent = self.GetParent<Unit>().GetComponent<NumericComponent>();

            int MaxHp = numericComponent.GetAsInt(NumericType.MaxHp);
            int Hp    = numericComponent.GetAsInt(NumericType.Hp);

            self.HpText.text = $"{Hp} / {MaxHp}";
            self.HpBar.size = new Vector2((float)Hp / MaxHp, self.HpBar.size.y);
        }
        


    }
}