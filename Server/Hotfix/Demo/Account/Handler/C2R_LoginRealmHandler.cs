using System;

namespace ET
{
    public class C2R_LoginRealmHandler: AMRpcHandler<C2R_LoginRealm, R2C_LoginRealm>
    {
        protected override async ETTask Run(Session session, C2R_LoginRealm request, R2C_LoginRealm response, Action reply)
        {
            if (  SceneType.Realm != session.DomainScene().SceneType )
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }
            Scene domainScene = session.DomainScene();
            
            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                reply();
                session?.Disconnect().Coroutine();
                return;
            }

            string token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);

            if (token == null || token != request.RealmTokenKey)
            {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session?.Disconnect().Coroutine();
                return;
            }
            
            domainScene.GetComponent<TokenComponent>().Remove(request.AccountId);
            
            
            using (session.AddComponent<SessionLockingComponent>())
            using (await CoroutineLockComponent.Instance.Wait((int)CoroutineLockType.LoginRealm, request.AccountId))
            {
                // 取模固定分配一个Gate
                StartSceneConfig config = RealmGateAddressHelper.GetGate(domainScene.Zone,request.AccountId);
                
                // 向gate请求一个key,客户端可以拿着这个key连接gate
                G2R_GetLoginGateKey g2RGetLoginKey = (G2R_GetLoginGateKey) await MessageHelper.CallActor(config.InstanceId, new R2G_GetLoginGateKey() {AccountId = request.AccountId});
                
                if (g2RGetLoginKey.Error != ErrorCode.ERR_Success)
                {
                    response.Error   = g2RGetLoginKey.Error;
                    reply();
                    return;
                }
                
                response.GateSessionKey = g2RGetLoginKey.GateSessionKey;
                response.GateAddress    = config.OuterIPPort.ToString();
                reply();
                
                session?.Disconnect().Coroutine();
            }
        }
    }
}