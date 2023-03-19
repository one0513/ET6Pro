using DG.Tweening;
using ET.UIEventType;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(UIFrontTipComponent))]
    public class ShowFrontTipEvent_CreateFrontTipUI : AEventAsync<UIEventType.ShowFront>
    {
        protected override async ETTask Run(ShowFront args)
        {
            await Show(args.Text);
        }
        
        async ETTask Show(string Content, int seconds = 2)
        {
            GameObject gameObject = await GameObjectPoolComponent.Instance.GetGameObjectAsync(UIFrontTip.PrefabPath);
            UIFrontTip ui = UIFrontTipComponent.Instance.AddChild<UIFrontTip>();
            var transform = gameObject.transform;
            ui.AddUIComponent<UITransform,Transform>("", transform);
            transform = gameObject.transform;
            transform.SetParent(UIFrontTipComponent.Instance.root);
            transform.localPosition = new Vector3(-300,-400,0);
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(1, 1, 1);
            UIWatcherComponent.Instance.OnCreate(ui);
            UIWatcherComponent.Instance.OnEnable(ui,Content);
            
            gameObject.transform.DOLocalMoveY(transform.localPosition.y + 800f, 2f).onComplete = () =>
            {
                ui.BeforeOnDestroy();
                UIWatcherComponent.Instance.OnDestroy(ui);
                GameObjectPoolComponent.Instance.RecycleGameObject(gameObject);
            };
            
        }
    }
}