using System;

namespace ET
{
    [ActorMessageHandler]
    public class A2L_LoginAccountRequestHandler : AMActorRpcHandler<Scene,A2L_LoginAccountRequest,L2A_LoginAccountResponse>
    {
        protected override async ETTask Run(Scene scene, A2L_LoginAccountRequest request, L2A_LoginAccountResponse response, Action reply)
        {
            long accountId = request.AccountId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock,accountId.GetHashCode()))
            {
                if (!scene.GetComponent<LoginInfoRecordComponent>().IsExits(accountId))
                {
                    reply();
                    return;
                }

                int zone = scene.GetComponent<LoginInfoRecordComponent>().Get(accountId);
                StartSceneConfig sceneConfig = RealmGateAddressHelper.GetGate(zone,accountId);

                G2L_DisconnectGateUnit g2LDisconnectGateUnit = (G2L_DisconnectGateUnit) await MessageHelper.CallActor(sceneConfig.InstanceId, new L2G_DisconnectGateUnit() { AccountId = accountId });

                response.Error = g2LDisconnectGateUnit.Error;
                reply();

            }
        }
    }
}