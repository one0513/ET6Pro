using System;

namespace ET
{
    [FriendClass(typeof(SessionPlayerComponent))]
    public class L2G_DisconnectGateUnitHandler : AMActorRpcHandler<Scene,L2G_DisconnectGateUnit,G2L_DisconnectGateUnit>
    {
        protected override async ETTask Run(Scene scene, L2G_DisconnectGateUnit request, G2L_DisconnectGateUnit response, Action reply)
        {
            long accountId = request.AccountId;

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate,accountId.GetHashCode()))
            {
                PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();
                Player player = playerComponent.Get(accountId);

                if (player == null)
                {
                    reply();
                    return;
                }

                scene.GetComponent<GateSessionKeyComponent>().Remove(accountId);
                Session gateSession = player.ClientSession; 
                if ( gateSession!= null && !gateSession.IsDisposed)
                {
                    if (gateSession.GetComponent<SessionPlayerComponent>() != null)
                    {
                        gateSession.GetComponent<SessionPlayerComponent>().isLoginAgain = true;
                    }
                    
                    gateSession.Send(new A2C_Disconnect() { Error = ErrorCode.ERR_OtherAccountLogin});
                    gateSession?.Disconnect().Coroutine();
                }
                player.AddComponent<PlayerOfflineOutTimeComponent>();
            }
            reply();
        }
    }
}