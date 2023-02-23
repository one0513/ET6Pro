using System;
using System.Collections.Generic;
using System.Text;

namespace ET
{
    [Timer(TimerType.PlayerOfflineOutTime)]
    public class PlayerOfflineOutTime: ATimer<PlayerOfflineOutTimeComponent>
    {
        public override void Run(PlayerOfflineOutTimeComponent self)
        {
            try
            {
                self.KickPlayer();
            }
            catch (Exception e)
            {
                Log.Error($"playerOffline timer error: {self.Id}\n{e}");
            }
        }
    }
    
    
    [ObjectSystem]
    public class GateUnitDeleteComponentDestroySystem : DestroySystem<PlayerOfflineOutTimeComponent>
    {
        public override void Destroy(PlayerOfflineOutTimeComponent self)
        {
            TimerComponent.Instance.Remove(ref self.Timer);
        }
    }

    [ObjectSystem]
    public class PlayerOfflineOutTimeComponentAwakeSystem : AwakeSystem<PlayerOfflineOutTimeComponent>
    {
        public override void Awake(PlayerOfflineOutTimeComponent self)
        {
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 10000,TimerType.PlayerOfflineOutTime,self);
        }
    }

    public static class GateUnitDeleteComponentSystem
    {
        public static void KickPlayer(this PlayerOfflineOutTimeComponent self)
        {
            DisconnectHelper.KickPlayer(self.GetParent<Player>()).Coroutine();
        }
    }


}