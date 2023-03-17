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
	        List<UnitInfo> unitInfos = new List<UnitInfo>();
	        M2C_InitMonsterInfoList m2CInitMonsterInfoList = new M2C_InitMonsterInfoList();
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
			
	            unit.AddComponent<MoveComponent>();
	            unit.AddComponent<PathfindingComponent, string>("Map");

	            float x =  unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.PosX);
	            float z =  unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.PosZ);
	            unit.Position = new Vector3(x, 0, z);
			
	            unit.AddComponent<MailBoxComponent>();
		
	            // 通知客户端创建My Unit
	            M2C_CreateMyUnit m2CCreateUnits = new M2C_CreateMyUnit();
	            m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
	            MessageHelper.SendToClient(unit, m2CCreateUnits);

	            //初始化 小队内的Monster Unit
	            long roomId = unit.GetComponent<NumericComponent>().GetAsLong(NumericType.RoomID);
	            if (roomId != 0)
	            {
		            List<long> monsterIds = scene.GetComponent<MonsterFactoryComponent>().Get(roomId);
		            unitInfos.Clear();
		            if (monsterIds != null && monsterIds.Count > 0)
		            {
			            for (int i = 0; i < monsterIds.Count; i++)
			            {
				            unitInfos.Add(UnitHelper.CreateUnitInfo(scene.GetComponent<UnitComponent>().Get(monsterIds[i])));
			            }
		            }
		            else
		            {
			            for (int i = 0; i < 3; i++)
			            {
				            Unit monster = scene.GetComponent<MonsterFactoryComponent>().CreateMonsterUnit(roomId);

				            monster.AddComponent<BattleUnitFindComponent, long>(roomId);
				            monster.Position = new Vector3(i + 1, 0, i + 1);
				            unitInfos.Add(UnitHelper.CreateUnitInfo(monster));
			            }
		            }
      
		            m2CInitMonsterInfoList.UnitList = unitInfos;
		            MessageHelper.SendToClient(unit, m2CInitMonsterInfoList);
		            
		            
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
					//////////
		            
	            }
	            
	           
				
				//通知客户端同步背包信息
				//ItemUpdateNoticeHelper.SyncAllBagItems(unit);
				//ItemUpdateNoticeHelper.SyncAllEquipItems(unit);

				//通知客户端同步打造信息
				//ForgeHelper.SyncAllProduction(unit);
			
				TaskNoticeHelper.SyncAllTaskInfo(unit);
			
				unit.AddComponent<NumericNoticeComponent>();
				unit.AddComponent<CombatUnitComponent>();
            
				unit.AddComponent<AdventureCheckComponent>();
			
			
				// 加入aoi 不是开发地图 不使用aoi
				// var aoiu = unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType,int>
				// 		(unit.Position,unit.Rotation,unit.Type,5);
				//
				// aoiu.AddSphereCollider(0.5f);
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
	            
	            //初始化地图怪物
	            long roomId = unit.GetComponent<NumericComponent>().GetAsLong(NumericType.RoomID);
	            if (roomId != 0)
	            {
		            List<long> monsterIds = scene.GetComponent<MonsterFactoryComponent>().Get(roomId);
		            unitInfos.Clear();
		            if (monsterIds != null && monsterIds.Count > 0)
		            {
			            for (int i = 0; i < monsterIds.Count; i++)
			            {
				            unitInfos.Add(UnitHelper.CreateUnitInfo(scene.GetComponent<UnitComponent>().Get(monsterIds[i])));
			            }
		            }
		            else
		            {
			            for (int i = 0; i < 3; i++)
			            {
				            Unit monster = scene.GetComponent<MonsterFactoryComponent>().CreateMonsterUnit(roomId);
				            monster.AddComponent<BattleUnitFindComponent, long>(roomId);
				            monster.Position = new Vector3(i + 1, 0, i + 1);
				            unitInfos.Add(UnitHelper.CreateUnitInfo(monster));
			            }
		            }
		            m2CInitMonsterInfoList.UnitList = unitInfos;
		            MessageHelper.SendToClient(unit, m2CInitMonsterInfoList);
		            
		            
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
		            
	            }
	            
	           
	            
            }
            
            reply();
        }
    }
}