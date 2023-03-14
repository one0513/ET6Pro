﻿

using System.Collections.Generic;

namespace ET
{
    [FriendClass(typeof(UnitGateComponent))]
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
        //简易版广播 不考虑aoi
        public static void BroadcastSomeUnit(List<Unit> units, IActorMessage message)
        {
            foreach (Unit unit in units)
            {
                SendToClient(unit, message);
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