using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [ActorMessageHandler]
    public class M2M_UnitTransferRequestHandler : AMActorRpcHandler<Scene, M2M_UnitTransferRequest, M2M_UnitTransferResponse>
    {
        protected override async ETTask Run(Scene scene, M2M_UnitTransferRequest request, M2M_UnitTransferResponse response, Action reply)
        {
            await ETTask.CompletedTask;
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            //初始化地图怪物
            List<UnitInfo> unitInfos = new List<UnitInfo>();
            M2C_InitMonsterInfoList m2CInitMonsterInfoList = new M2C_InitMonsterInfoList();
            
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

	            List<long> monsterIds = scene.GetComponent<MonsterFactoryComponent>().Get(unit.Id);
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
					for (int i = 0; i < 1; i++)
					{
						Unit monster = scene.GetComponent<MonsterFactoryComponent>().CreateMonsterUnit(unit.Id);
						monster.Position = new Vector3(i + 1, 0, i + 1);
						unitInfos.Add(UnitHelper.CreateUnitInfo(monster));
					}
				}
      
				m2CInitMonsterInfoList.UnitList = unitInfos;
				MessageHelper.SendToClient(unit, m2CInitMonsterInfoList);

				await TimerComponent.Instance.WaitAsync(1000);
				List<long> monsterId = scene.GetComponent<MonsterFactoryComponent>().Get(unit.Id);
				for (int i = 0; i < monsterId.Count; i++)
				{
					scene.GetComponent<UnitComponent>().Get(monsterId[i]).AddComponent<ZhuiZhuAimComponent, Unit, Action>(unit, () =>
					{
		            
					});

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
			
			
				// 加入aoi
				var aoiu = unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType,int>
						(unit.Position,unit.Rotation,unit.Type,5);
			
				aoiu.AddSphereCollider(0.5f);
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
	            List<long> monsterIds = scene.GetComponent<MonsterFactoryComponent>().Get(unit.Id);
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
			            unitInfos.Add(UnitHelper.CreateUnitInfo(scene.GetComponent<MonsterFactoryComponent>().CreateMonsterUnit(unit.Id)));
		            }
	            }
	            m2CInitMonsterInfoList.UnitList = unitInfos;
	            MessageHelper.SendToClient(unit, m2CInitMonsterInfoList);
            }
            
            reply();
        }
    }
}