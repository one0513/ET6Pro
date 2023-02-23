using System;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(RouterDataComponent))]
    [FriendClass(typeof(GetRouterComponent))]
    [FriendClass(typeof(ServerInfosComponent))]
    [FriendClass(typeof(RoleInfosComponent))]
    [FriendClass(typeof(RoleInfo))]
    public static class LoginHelper
    {
        [Timer(TimerType.LoginTimeOut)]
        public class LoginTimeOut: ATimer<ETCancellationToken>
        {
            public override void Run(ETCancellationToken cancel)
            {
                try
                {
                    cancel.Cancel();
                    Log.Info("Login Time Out");
                }
                catch (Exception e)
                {
                    Log.Error($"move timer error: LoginTimeOut\n{e}");
                }
            }
        }

        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password, Action onError = null)
        {
            A2C_LoginAccount a2CLoginAccount = null;
            Session accountSession = null;
            try
            {
                accountSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                password = MD5Helper.StringMD5(password);
                a2CLoginAccount = (A2C_LoginAccount)await accountSession.Call(new C2A_LoginAccount() { AccountName = account, Password = password });
            }
            catch (Exception e)
            {
                accountSession?.Dispose();
                Log.Error(e.ToString());
                onError?.Invoke();
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2CLoginAccount.Error != ErrorCode.ERR_Success)
            {
                accountSession?.Dispose();
                onError?.Invoke();
                return a2CLoginAccount.Error;
            }

            zoneScene.AddComponent<SessionComponent>().Session = accountSession;
            zoneScene.GetComponent<SessionComponent>().Session.AddComponent<PingComponent>();

            zoneScene.GetComponent<AccountInfoComponent>().token = a2CLoginAccount.Token;
            zoneScene.GetComponent<AccountInfoComponent>().accountId = a2CLoginAccount.AccountId;
            
            return ErrorCode.ERR_Success;
        }


        public static async ETTask<int> GetServerInfos(Scene zoneScene)
        {
            A2C_GetServerInfos a2CGetServerInfos = null;
            try
            {
                a2CGetServerInfos = (A2C_GetServerInfos)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetServerInfos()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().accountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().token
                            
                });
            }
            catch (Exception e)
            {
               Log.Error(e.ToString());
               return ErrorCode.ERR_NetWorkError;
            }

            if (a2CGetServerInfos.Error != ErrorCode.ERR_Success)
            {
                return a2CGetServerInfos.Error;
            }

            foreach (var serverInfo in a2CGetServerInfos.ServerInfoList)
            {
                ServerInfo info = zoneScene.GetComponent<ServerInfosComponent>().AddChild<ServerInfo>();
                info.FromMessage(serverInfo);
                zoneScene.GetComponent<ServerInfosComponent>().Add(info);
            }

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;

        }

        public static async ETTask<int> AutoCreaterOrGetRole(Scene zoneScene)
        {
            RoleInfo info = new RoleInfo();
            A2C_GetRoles a2CGetRoles = null;
            a2CGetRoles = (A2C_GetRoles) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRoles()
            {
                AccountId = zoneScene.GetComponent<AccountInfoComponent>().accountId,
                Token = zoneScene.GetComponent<AccountInfoComponent>().token,
                ServerId = zoneScene.GetComponent<ServerInfosComponent>().curServerId
            });

            if (a2CGetRoles.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2CGetRoles.Error.ToString());
                return a2CGetRoles.Error;
            }
            
            A2C_CreateRole a2CCreateRole = null;
            if (a2CGetRoles.RoleInfo.Count > 0)
            {
                info.FromMessage(a2CGetRoles.RoleInfo[0]);
                zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.Add(info);
                zoneScene.GetComponent<RoleInfosComponent>().CurrentRoleId = info.Id;
                
                Log.Debug($"当前区服玩家名：{info.Name}");
                return ErrorCode.ERR_Success;
            }
            else
            {
                a2CCreateRole = (A2C_CreateRole) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_CreateRole()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().accountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().curServerId,
                    Name = "新玩家",
                });
            }

            if (a2CCreateRole != null && a2CCreateRole.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2CCreateRole.Error.ToString());
                return a2CCreateRole.Error;
            }

            
            info.FromMessage(a2CCreateRole.RoleInfo);
            zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.Add(info);
            zoneScene.GetComponent<RoleInfosComponent>().CurrentRoleId = info.Id;
            
            Log.Debug($"当前区服玩家名：{info.Name}");
            return ErrorCode.ERR_Success;
        }


        public static async ETTask<int> GetRealmKey(Scene zoneScene)
        {
            A2C_GetRealmKey a2CGetRealmKey = null;
            try
            {
                a2CGetRealmKey = (A2C_GetRealmKey)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRealmKey()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().accountId,
                    Token =  zoneScene.GetComponent<AccountInfoComponent>().token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().curServerId
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }
            
            if (a2CGetRealmKey.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2CGetRealmKey.Error.ToString());
                return a2CGetRealmKey.Error;
            }

            zoneScene.GetComponent<AccountInfoComponent>().realmKey = a2CGetRealmKey.RealmKey;
            zoneScene.GetComponent<AccountInfoComponent>().realmAddress = a2CGetRealmKey.RealmAddress;
            zoneScene.GetComponent<SessionComponent>().Session.Dispose();
            
            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }
        
        
        public static async ETTask<int> EnterGame(Scene zoneScene)
        {
            string realmAddress = zoneScene.GetComponent<AccountInfoComponent>().realmAddress;
            // 1. 连接Realm，获取分配的Gate
            R2C_LoginRealm r2CLogin;

            Session session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(realmAddress));
            try
            {
                
                r2CLogin = (R2C_LoginRealm) await session.Call(new C2R_LoginRealm() 
                                                {    
                                                    AccountId =  zoneScene.GetComponent<AccountInfoComponent>().accountId,
                                                    RealmTokenKey = zoneScene.GetComponent<AccountInfoComponent>().realmKey 
                                                });
            }
            catch (Exception e)
            {
                Log.Error(e);
                session?.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }
            session?.Dispose();


            if (r2CLogin.Error != ErrorCode.ERR_Success)
            {
                return r2CLogin.Error;
            }
            
            Log.Warning($"GateAddress :  {r2CLogin.GateAddress}");
            Session gateSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(r2CLogin.GateAddress));
            gateSession.AddComponent<PingComponent>();
            zoneScene.GetComponent<SessionComponent>().Session = gateSession;
            
            long currentRoleId = zoneScene.GetComponent<RoleInfosComponent>().CurrentRoleId;
            // 2. 开始连接Gate
            G2C_LoginGameGate g2CLoginGate = null;
            try
            {
                long accountId = zoneScene.GetComponent<AccountInfoComponent>().accountId;
                //需修改 roleid相关逻辑
                 g2CLoginGate = (G2C_LoginGameGate)await gateSession.Call(new C2G_LoginGameGate() { Key = r2CLogin.GateSessionKey, Account = accountId,RoleId = currentRoleId});
                
            }
            catch (Exception e)
            {
                Log.Error(e);
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }
            
            if (g2CLoginGate.Error != ErrorCode.ERR_Success)
            {
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return g2CLoginGate.Error;
            }
            Log.Debug("登陆gate成功!");

            //3. 角色正式请求进入游戏逻辑服
            G2C_EnterGame g2CEnterGame = null;
            try
            {
   
                g2CEnterGame = (G2C_EnterGame)await gateSession.Call(new C2G_EnterGame() { });
            }
            catch (Exception e)
            {
                Log.Error(e);
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }

            if (g2CEnterGame.Error != ErrorCode.ERR_Success)
            {
                Log.Error(g2CEnterGame.Error.ToString());
                return g2CEnterGame.Error;
            }
            
            Log.Debug("角色进入游戏成功!!!!");
            zoneScene.GetComponent<PlayerComponent>().MyId = g2CEnterGame.MyId;
            
            await zoneScene.GetComponent<ObjectWait>().Wait<WaitType.Wait_SceneChangeFinish>();
            
            return ErrorCode.ERR_Success;
        }

    }
}