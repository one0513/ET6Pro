

namespace ET
{
	public class LoginFinish_CreateLobbyUI: AEventAsync<EventType.LoginFinish>
	{
		protected override async ETTask Run(EventType.LoginFinish args)
		{
			await ETTask.CompletedTask;
			// args.ZoneScene.GetComponent<PlayerComponent>().Account = args.Account;
			// await UIManagerComponent.Instance.OpenWindow<UILobbyView,Scene>(UILobbyView.PrefabPath,args.ZoneScene);
		}
	}
}
