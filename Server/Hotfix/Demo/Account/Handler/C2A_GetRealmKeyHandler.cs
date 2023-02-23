using System;

namespace ET
{
    public  class C2A_GetRealmKeyHandler : AMRpcHandler<C2A_GetRealmKey,A2C_GetRealmKey>
    {
        protected override async ETTask Run(Session session, C2A_GetRealmKey request, A2C_GetRealmKey response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)
            {
                Log.Error($"账号请求Scene 错误，当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }
            
            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            string token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);

            if (token == null || token != request.Token)
            {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.AccountId))
                {
                    StartSceneConfig realmStartSceneConfig = RealmGateAddressHelper.GetRealm(request.ServerId);

                    R2A_GetRealmKey r2AGetRealmKey =  (R2A_GetRealmKey) await MessageHelper.CallActor(realmStartSceneConfig.InstanceId, new A2R_GetRealmKey() { AccountId = request.AccountId });

                    if (r2AGetRealmKey.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = r2AGetRealmKey.Error;
                        reply();
                        session?.Disconnect().Coroutine();
                        return;
                    }

                    response.RealmKey = r2AGetRealmKey.RealmKey;
                    response.RealmAddress = realmStartSceneConfig.OuterIPPort.ToString();
                    reply();
                    session?.Disconnect().Coroutine();
                    
                }
            }



            await ETTask.CompletedTask;
        }
    }
}