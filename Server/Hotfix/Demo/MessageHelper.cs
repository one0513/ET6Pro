

using System.Collections.Generic;

namespace ET
{
    [FriendClass(typeof(UnitGateComponent))]
    [FriendClass(typeof(RoomInfo))]
    public static class MessageHelper
    {
        public static void Broadcast(Unit unit, IActorMessage message,GhostComponent ghost =null)
        {
            if (ghost == null)
            {
                ghost = unit.GetComponent<AOIUnitComponent>()?.GetComponent<GhostComponent>();
            }
            if (ghost!=null && !ghost.IsGoast)
            {
                unit.GetComponent<AOIUnitComponent>()?.GetComponent<GhostComponent>()?.HandleMsg(message);
            }
            
            foreach (var u in unit.GetBeSeeUnits())
            {
                SendToClient(u.GetParent<Unit>(), message);
            }
        }
        //简易版广播 小队广播
        public static async ETTask RoomBroadcast(Unit unit, IActorMessage message)
        {
            long roomId = unit.GetComponent<NumericComponent>().GetAsLong(NumericType.RoomID);

            if (roomId == 0)
            {
                SendToClient(unit, message);
            }
            else
            {
                RoomInfo info = await unit.DomainScene().GetComponent<RoomInfoComponent>().Get(roomId);
                if (info != null)
                {
                    for (int i = 0; i < info.playerList.Count; i++)
                    {
                        Unit onlineUnit = unit.DomainScene().GetComponent<UnitComponent>().Get(info.playerList[i]);
                        if (onlineUnit != null)
                        {
                            SendToClient(onlineUnit, message);
                        }
                    }
                }
                else
                {
                    var infos = await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoomInfo>(d => d.RoomId == roomId);
                    for (int i = 0; i < infos[0].playerList.Count; i++)
                    {
                        Unit onlineUnit = unit.DomainScene().GetComponent<UnitComponent>().Get(infos[0].playerList[i]);
                        if (onlineUnit != null)
                        {
                            SendToClient(onlineUnit, message);
                        }
                    }
                }
            }
        }
        
        public static void SendToClient(Unit unit, IActorMessage message)
        {
            SendActor(unit.GetComponent<UnitGateComponent>().GateSessionActorId, message);
        }
        
        
        /// <summary>
        /// 发送协议给ActorLocation
        /// </summary>
        /// <param name="id">注册Actor的Id</param>
        /// <param name="message"></param>
        public static void SendToLocationActor(long id, IActorLocationMessage message)
        {
            ActorLocationSenderComponent.Instance.Send(id, message);
        }
        
        /// <summary>
        /// 发送协议给Actor
        /// </summary>
        /// <param name="actorId">注册Actor的InstanceId</param>
        /// <param name="message"></param>
        public static void SendActor(long actorId, IActorMessage message)
        {
            ActorMessageSenderComponent.Instance.Send(actorId, message);
        }
        
        /// <summary>
        /// 发送RPC协议给Actor
        /// </summary>
        /// <param name="actorId">注册Actor的InstanceId</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async ETTask<IActorResponse> CallActor(long actorId, IActorRequest message)
        {
            return await ActorMessageSenderComponent.Instance.Call(actorId, message);
        }
        
        /// <summary>
        /// 发送RPC协议给ActorLocation
        /// </summary>
        /// <param name="id">注册Actor的Id</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async ETTask<IActorResponse> CallLocationActor(long id, IActorLocationRequest message)
        {
            return await ActorLocationSenderComponent.Instance.Call(id, message);
        }
    }
}