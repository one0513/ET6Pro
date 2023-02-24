using System;

namespace ET
{
    [FriendClass(typeof(SessionStateComponent))]
    [FriendClass(typeof(SessionPlayerComponent))]
    //[FriendClassAttribute(typeof(ET.RoleInfo))]
    [FriendClassAttribute(typeof(ET.UnitGateComponent))]
    public class C2G_EnterGameHandler : AMRpcHandler<C2G_EnterGame, G2C_EnterGame>
    {
        protected override async ETTask Run(Session session, C2G_EnterGame request, G2C_EnterGame response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Gate)
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }

            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                reply();
                return;
            }

            SessionPlayerComponent sessionPlayerComponent = session.GetComponent<SessionPlayerComponent>();
            if (null == sessionPlayerComponent)
            {
                response.Error = ErrorCode.ERR_SessionPlayerError;
                reply();
                return;
            }

            Player player = Game.EventSystem.Get(sessionPlayerComponent.PlayerInstanceId) as Player;

            if (player == null || player.IsDisposed)
            {
                response.Error = ErrorCode.ERR_NonePlayerError;
                reply();
                return;
            }

            long instanceId = session.InstanceId;

            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, player.AccountId.GetHashCode()))
                {

                    if (instanceId != session.InstanceId || player.IsDisposed)
                    {
                        response.Error = ErrorCode.ERR_PlayerSessionError;
                        reply();
                        return;
                    }

                    if (session.GetComponent<SessionStateComponent>() != null && session.GetComponent<SessionStateComponent>().State == SessionState.Game)
                    {
                        response.Error = ErrorCode.ERR_SessionStateError;
                        reply();
                        return;
                    }


                    if (player.PlayerState == PlayerState.Game)
                    {
                        try
                        {
                            IActorResponse reqEnter = await MessageHelper.CallLocationActor(player.UnitId, new G2M_RequestEnterGameState());
                            if (reqEnter.Error == ErrorCode.ERR_Success)
                            {
                                Unit unit= await UnitHelper.GetUnitFromCache(player);

                                //unit.AddComponent<UnitGateComponent, long>(session.InstanceId);
                                unit.AddComponent<UnitGateComponent, long>(player.InstanceId);
                        
                                //player.ChatInfoInstanceId = await this.EnterWorldChatServer(unit); //登录聊天服
                        
                                //玩家Unit上线后的初始化操作
                                await UnitHelper.InitUnit(unit, true);
                                response.MyId = unit.Id;
                                reply();
                                
                                try
                                {
                                    StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Games[player.DomainZone()];
                                    await TransferHelper.Transfer(unit, startSceneConfig.InstanceId, startSceneConfig.Name);
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e.ToString());
                                }
                                
                                return;
                            }
                            Log.Error("二次登录失败  " + reqEnter.Error + " | " + reqEnter.Message);
                            response.Error = ErrorCode.ERR_ReEnterGameError;
                            await DisconnectHelper.KickPlayer(player, true);
                            reply();
                            session?.Disconnect().Coroutine();
                        }
                        catch (Exception e)
                        {
                            Log.Error("二次登录失败  " + e.ToString());
                            response.Error = ErrorCode.ERR_ReEnterGameError2;
                            await DisconnectHelper.KickPlayer(player, true);
                            reply();
                            session?.Disconnect().Coroutine();
                            throw;
                        }
                        return;
                    }

                    try
                    {

                        //GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
                        //gateMapComponent.Scene = await SceneFactory.Create(gateMapComponent, "GateMap", SceneType.Map);

                        //从数据库或者缓存中加载出Unit实体及其相关组件
                        (bool isNewPlayer, Unit unit) = await UnitHelper.LoadUnit(player);

                        //unit.AddComponent<UnitGateComponent, long>(session.InstanceId);
                        unit.AddComponent<UnitGateComponent, long>(player.InstanceId);
                        
                        //player.ChatInfoInstanceId = await this.EnterWorldChatServer(unit); //登录聊天服
                        
                        //玩家Unit上线后的初始化操作
                        await UnitHelper.InitUnit(unit, isNewPlayer);
                        response.MyId = unit.Id;
                        reply();

                        StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Games[player.DomainZone()];
                        await TransferHelper.Transfer(unit, startSceneConfig.InstanceId, startSceneConfig.Name);
                        
                        SessionStateComponent SessionStateComponent = session.GetComponent<SessionStateComponent>();
                        if (SessionStateComponent == null)
                        {
                            SessionStateComponent = session.AddComponent<SessionStateComponent>();
                        }
                        SessionStateComponent.State = SessionState.Game;

                        player.PlayerState = PlayerState.Game;
                    }
                    catch (Exception e)
                    {

                        Log.Error($"角色进入游戏逻辑服出现问题 账号Id: {player.AccountId}  角色Id: {player.Id}   异常信息： {e.ToString()}");
                        response.Error = ErrorCode.ERR_EnterGameError;
                        reply();
                        await DisconnectHelper.KickPlayer(player, true);
                        session.Disconnect().Coroutine();

                    }
                }
            }
        }
        
        // private async ETTask<long> EnterWorldChatServer(Unit unit)
        // {
        //     StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(unit.DomainZone(), "ChatInfo");
        //     Chat2G_EnterChat chat2GEnterChat = (Chat2G_EnterChat)await MessageHelper.CallActor(startSceneConfig.InstanceId, new G2Chat_EnterChat()
        //     {
        //         UnitId           = unit.Id,
        //         Name             = unit.GetComponent<RoleInfo>().Name,
        //         GateSessionActorId = unit.GetComponent<UnitGateComponent>().GateSessionActorId
        //     });
        //     
        //     return chat2GEnterChat.ChatInfoUnitInstanceId;
        //     
        // }
    }
}