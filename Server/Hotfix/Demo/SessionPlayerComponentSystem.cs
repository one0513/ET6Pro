

namespace ET
{
	[FriendClass(typeof(SessionPlayerComponent))]
	public static class SessionPlayerComponentSystem
	{
		public class SessionPlayerComponentDestroySystem: DestroySystem<SessionPlayerComponent>
		{
			public override void Destroy(SessionPlayerComponent self)
			{
				// 发送断线消息
				if (!self.isLoginAgain && self.PlayerInstanceId != 0)
				{
					Player player = Game.EventSystem.Get(self.PlayerInstanceId) as Player;
					DisconnectHelper.KickPlayer(player).Coroutine();
				}

				self.AccountId = 0;
				self.PlayerId = 0;
				self.PlayerInstanceId = 0;
				self.isLoginAgain = false;
			}
		}

		public static Player GetMyPlayer(this SessionPlayerComponent self)
		{
			return self.Domain.GetComponent<PlayerComponent>().Get(self.PlayerId);
		}
	}
}
