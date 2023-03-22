using System;

namespace ET
{
    public class C2M_UnloadEquipItemHandler : AMActorLocationRpcHandler<Unit,C2M_UnloadEquipItem,M2C_UnloadEquipItem>
    {
        protected override async ETTask Run(Unit unit, C2M_UnloadEquipItem request, M2C_UnloadEquipItem response, Action reply)
        {
            BagComponent bagComponent               = unit.GetComponent<BagComponent>();
            EquipmentsComponent equipmentsComponent = unit.GetComponent<EquipmentsComponent>();
            
            if (bagComponent.IsMaxLoad())
            {
                response.Error = ErrorCode.ERR_BagMaxLoad;
                reply();
                return;
            }

            if (!equipmentsComponent.IsEquipItemByPosition((EquipPosition)request.EquipPosition))
            {
                response.Error = ErrorCode.ERR_ItemNotExist;
                reply();
                return;
            }
            
            
            Item equipItem = equipmentsComponent.GetEquipItemByPosition((EquipPosition)request.EquipPosition);

            if ( !bagComponent.IsCanAddItem(equipItem) )
            {
                response.Error = ErrorCode.ERR_AddBagItemError;
                reply();
                return;
            }
            
            equipItem = equipmentsComponent.UnloadEquipItemByPosition((EquipPosition)request.EquipPosition);

            bagComponent.AddItem(equipItem);
            
            reply();
            
            await ETTask.CompletedTask;
        }
    }
}