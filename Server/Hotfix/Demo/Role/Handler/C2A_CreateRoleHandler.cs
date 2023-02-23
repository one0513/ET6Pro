using System;

namespace ET
{
    [FriendClass(typeof(RoleInfo))]
    public class C2A_CreateRoleHandler : AMRpcHandler<C2A_CreateRole,A2C_CreateRole>
    {
        protected override async ETTask Run(Session session, C2A_CreateRole request, A2C_CreateRole response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
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
                session?.Disconnect().Coroutine();
                return;
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                //response.Error = ErrorCode.ERR_RoleNameIsNull;
                reply();
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CreateRole,request.AccountId))
                {
                    // var roleInfos = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone())
                    //         .Query<RoleInfo>(d => d.Name == request.Name && d.ServerId == request.ServerId && d.State != (int)RoleInfoState.Freeze);

                    
                    var roleInfos = await DBManagerComponent.Instance.GetZoneDB(request.ServerId)
                            .Query<RoleInfo>(d => d.AccountId == request.AccountId && d.ServerId == request.ServerId && d.State != (int)RoleInfoState.Freeze);
                    if (roleInfos != null && roleInfos.Count > 0)
                    {
                        response.Error = ErrorCode.ERR_HasCreatedRole;
                        reply();
                        return;
                    }

                    RoleInfo newRoleInfo = session.GetComponent<RoleInfosZone>().AddChildWithId<RoleInfo>(IdGenerater.Instance.GenerateUnitId(request.ServerId));
                    var allroleInfos = await DBManagerComponent.Instance.GetZoneDB(request.ServerId)
                            .Query<RoleInfo>(d => d.ServerId == request.ServerId);
                    
                    newRoleInfo.Name = "新玩家" + (allroleInfos.Count+1);
                    newRoleInfo.State = (int)RoleInfoState.Normal;
                    newRoleInfo.ServerId = request.ServerId;
                    newRoleInfo.AccountId = request.AccountId;
                    newRoleInfo.CreateTime = TimeHelper.ServerNow();
                    newRoleInfo.LastLoginTime = 0;

                    await DBManagerComponent.Instance.GetZoneDB(newRoleInfo.ServerId).Save<RoleInfo>(newRoleInfo);

                    response.RoleInfo = newRoleInfo.ToMessage();

                    reply();

                    newRoleInfo?.Dispose();

                }
            }
           
        }
    }
}