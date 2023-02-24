using System;
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
            Unit unit = request.Unit;

            // if (unitComponent.GetChild<Unit>(unit.Id) != null)
            // {
	           //  unitComponent.GetChild<Unit>(unit.Id).Dispose();
            // }
            unitComponent.GetChild<Unit>(unit.Id)?.Dispose();
            unitComponent.AddChild(unit);
            unitComponent.Add(unit);
            if (unit.GetComponent<UnitDBSaveComponent>() == null)
            {
	            unit.AddComponent<UnitDBSaveComponent>();
            }

            
            foreach (Entity entity in request.Entitys)
            {
                unit.AddComponent(entity);
            }
			
            //unit.AddComponent<MoveComponent>();
            //unit.AddComponent<PathfindingComponent, string>(scene.Name);
            //unit.Position = new Vector3(-10, 0, -10);
			
            unit.AddComponent<MailBoxComponent>();
		
            // 通知客户端创建My Unit
            M2C_CreateMyUnit m2CCreateUnits = new M2C_CreateMyUnit();
            m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
            MessageHelper.SendToClient(unit, m2CCreateUnits);
			
            //通知客户端同步背包信息
            //ItemUpdateNoticeHelper.SyncAllBagItems(unit);
            //ItemUpdateNoticeHelper.SyncAllEquipItems(unit);

            //通知客户端同步打造信息
            //ForgeHelper.SyncAllProduction(unit);
			
            //TaskNoticeHelper.SyncAllTaskInfo(unit);
			
            unit.AddComponent<NumericNoticeComponent>();
            //unit.AddComponent<AdventureCheckComponent>();
			
			
			
			
            // 加入aoi
            //unit.AddComponent<AOIEntity, int, Vector3>(9 * 1000, unit.Position);

            response.NewInstanceId = unit.InstanceId;
			
            reply();
        }
    }
}