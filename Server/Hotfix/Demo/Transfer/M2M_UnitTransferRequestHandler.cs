using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [ActorMessageHandler]
    [FriendClass(typeof(RoomInfo))]
    public class M2M_UnitTransferRequestHandler : AMActorRpcHandler<Scene, M2M_UnitTransferRequest, M2M_UnitTransferResponse>
    {
        protected override async ETTask Run(Scene scene, M2M_UnitTransferRequest request, M2M_UnitTransferResponse response, Action reply)
        {
            await ETTask.CompletedTask;
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            //初始化小队Unit
            M2C_InitOneRoomPlayerInfo m2CInitOneRoomPlayerInfo = new M2C_InitOneRoomPlayerInfo();
	        
            if (unitComponent.Get(request.Unit.Id) == null)
            {
	            Unit unit = request.Unit;
            
	            unitComponent.GetChild<Unit>(unit.Id)?.Dispose();
	            unitComponent.AddChild(unit);

	            if (unit.GetComponent<UnitDBSaveComponent>() == null)
	            {
		            unit.AddComponent<UnitDBSaveComponent>();
	            }

            
	            foreach (Entity entity in request.Entitys)
	            {
		            unit.AddComponent(entity);
	            }
			
	            // unit.AddComponent<MoveComponent>();
	            // unit.AddComponent<PathfindingComponent, string>("Map");
	            unit.AddComponent<AIComponent,int>(4);


	            var num = unit.GetComponent<NumericComponent>();
	            // float x =  num.GetAsFloat(NumericType.PosX);
	            // float z =  num.GetAsFloat(NumericType.PosZ);
	            unit.Position = new Vector3(0, 0, -2);
	            num.Set(NumericType.IsAlive,1);
	            num.Set(NumericType.Hp,num.GetAsInt(NumericType.MaxHp));
	            
			
	            unit.AddComponent<MailBoxComponent>();
		
	            // 通知客户端创建My Unit
	            M2C_CreateMyUnit m2CCreateUnits = new M2C_CreateMyUnit();
	            m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
	            MessageHelper.SendToClient(unit, m2CCreateUnits);

	            //初始化 小队内的Monster Unit
	            long roomId = unit.GetComponent<NumericComponent>().GetAsLong(NumericType.RoomID);
	            
	            
	            if (roomId != 0)
	            {
		            
		            //初始化 小队内的Player Unit
		            RoomInfo info =  await unit.DomainScene().GetComponent<RoomInfoComponent>().Get(roomId);
		            if (info != null)
		            {
			            if (info.RoomPlayerNum > 1)
			            {
				            foreach (var unitId in info.playerList)
				            {
					            if (unitId != unit.Id)
					            {
						            Unit onlineUnit = unitComponent.Get(unitId);
						            if (onlineUnit != null)
						            {
							            m2CInitOneRoomPlayerInfo.UnitInfo = UnitHelper.CreateUnitInfo(onlineUnit);
							            MessageHelper.SendToClient(unit, m2CInitOneRoomPlayerInfo);
						            }
					            }
				            }
			            }
		            }
		            else
		            {
			            var infos =  await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoomInfo>(d => d.RoomId == roomId);
					
			            if (infos[0].playerList.Count > 1 )
			            {
				            foreach (var unitId in infos[0].playerList)
				            {
					            if (unitId != unit.Id)
					            {
						            Unit onlineUnit = unitComponent.Get(unitId);
						            if (onlineUnit != null)
						            {
							            m2CInitOneRoomPlayerInfo.UnitInfo = UnitHelper.CreateUnitInfo(onlineUnit);
							            MessageHelper.SendToClient(unit, m2CInitOneRoomPlayerInfo);
						            }
					            }
				            }
			            }
		            }
		            
		            await scene.GetComponent<MonsterFactoryComponent>().InitRoomMonster(roomId,unit);
		            unit.AddComponent<BattleUnitFindComponent, long>(roomId);
	            }

	            
	            
				//通知客户端同步背包信息
				ItemUpdateNoticeHelper.SyncAllBagItems(unit);
				ItemUpdateNoticeHelper.SyncAllEquipItems(unit);
				
				BagHelper.AddItemByConfigId(unit,1011,100);
				BagHelper.AddItemByConfigId(unit,1012,100);
				BagHelper.AddItemByConfigId(unit,1013,100);
				BagHelper.AddItemByConfigId(unit,1014,100);
				BagHelper.AddItemByConfigId(unit,1015,100);
				BagHelper.AddItemByConfigId(unit,1016,100);

				//通知客户端同步打造信息
				//ForgeHelper.SyncAllProduction(unit);
			
				TaskNoticeHelper.SyncAllTaskInfo(unit);
			
				unit.AddComponent<NumericNoticeComponent>();
				unit.AddComponent<CombatUnitComponent>();
            
				unit.AddComponent<AdventureCheckComponent>();
				
				response.NewInstanceId = unit.InstanceId;
            }
            else
            {
	            Unit unit = unitComponent.Get(request.Unit.Id);
	            // 通知客户端创建My Unit
	            M2C_CreateMyUnit m2CCreateUnits = new M2C_CreateMyUnit();
	            m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
	            MessageHelper.SendToClient(unit, m2CCreateUnits);
	            response.NewInstanceId = unit.InstanceId;
	            
	            //通知客户端同步背包信息
	            ItemUpdateNoticeHelper.SyncAllBagItems(unit);
	            ItemUpdateNoticeHelper.SyncAllEquipItems(unit);

	            //通知客户端同步打造信息
	            //ForgeHelper.SyncAllProduction(unit);
			
	            TaskNoticeHelper.SyncAllTaskInfo(unit);
	            
	            //初始化地图怪物
	            long roomId = unit.GetComponent<NumericComponent>().GetAsLong(NumericType.RoomID);
	            if (roomId != 0)
	            {
		            //初始化 小队内的Player Unit
		            RoomInfo info =  await unit.DomainScene().GetComponent<RoomInfoComponent>().Get(roomId);
		            if (info != null)
		            {
			            if (info.RoomPlayerNum > 1)
			            {
				            foreach (var unitId in info.playerList)
				            {
					            if (unitId != unit.Id)
					            {
						            Unit onlineUnit = unitComponent.Get(unitId);
						            if (onlineUnit != null)
						            {
							            m2CInitOneRoomPlayerInfo.UnitInfo = UnitHelper.CreateUnitInfo(onlineUnit);
							            MessageHelper.SendToClient(unit, m2CInitOneRoomPlayerInfo);
						            }
						            
					            }
				            }
			            }
		            }
		            else
		            {
			            var infos =  await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoomInfo>(d => d.RoomId == roomId);
					
			            if (infos[0].playerList.Count > 1 )
			            {
				            foreach (var unitId in infos[0].playerList)
				            {
					            if (unitId != unit.Id)
					            {
						            Unit onlineUnit = unitComponent.Get(unitId);
						            if (onlineUnit != null)
						            {
							            m2CInitOneRoomPlayerInfo.UnitInfo = UnitHelper.CreateUnitInfo(onlineUnit);
							            MessageHelper.SendToClient(unit, m2CInitOneRoomPlayerInfo);
						            }
						            
					            }
				            }
			            }
		            }
		            await scene.GetComponent<MonsterFactoryComponent>().InitRoomMonster(roomId,unit);
		            
	            }
            }
            
            reply();
        }
    }
}