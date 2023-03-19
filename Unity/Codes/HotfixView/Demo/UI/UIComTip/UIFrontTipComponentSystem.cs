namespace ET
{
    
    [ObjectSystem]
    public class UIFrontTipComponentAwakeSystem : AwakeSystem<UIFrontTipComponent>
    {
        public override void Awake(UIFrontTipComponent self)
        {
            UIFrontTipComponent.Instance = self;
            self.root = UIManagerComponent.Instance.GetLayer(UILayerNames.TipLayer).transform;
        }
    }
    [ObjectSystem]
    public class UIFrontTipComponentDestroySystem : DestroySystem<UIFrontTipComponent>
    {
        public override void Destroy(UIFrontTipComponent self)
        {
            UIFrontTipComponent.Instance = null;
            self.root = null;
        }
    }
    

}