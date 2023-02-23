using System.Threading.Tasks;

namespace ET
{
    public class SceneChangeFinishEventAsyncCreateUIHelp : AEventAsync<EventType.SceneChangeFinish>
    {
        protected override async ETTask Run(EventType.SceneChangeFinish args)
        {
            await UIManagerComponent.Instance.CloseWindow<UILobbyView>();

            await UIManagerComponent.Instance.OpenWindow<UIMainView,Scene>(UIMainView.PrefabPath,args.CurrentScene);
            // args.CurrentScene.AddComponent<OperaComponent>();
            await Task.CompletedTask;
        }
    }
}
