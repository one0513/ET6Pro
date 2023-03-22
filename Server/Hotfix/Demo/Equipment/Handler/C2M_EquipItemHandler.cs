using System;


namespace ET
{
    public class C2M_EquipItemHandler : AMActorLocationRpcHandler<Unit,C2M_EquipItem,M2C_EquipItem>
    {
        protected override async ETTask Run(Unit unit, C2M_EquipItem request, M2C_EquipItem response, Action reply)
        {
            BagComponent bagComponent               = unit.GetComponent<BagComponent>();
            EquipmentsComponent equipmentsComponent = unit.GetComponent<EquipmentsComponent>();
            
            if (!bagComponent.IsItemExist(request.ItemUid))
            {
                response.Error = ErrorCode.ERR_ItemNotExist;
                reply();
                return;
            }

            Item bagItem      = bagComponent.GetItemById(request.ItemUid);
            var equipPosition = (EquipPosition)bagItem.Config.EquipPosition;
            bagItem           = bagComponent.RemoveItemNoDispose(bagItem);
            
            Item equipItem = equipmentsComponent.GetEquipItemByPosition(equipPosition);
            if (equipItem != null)
            {
                if ( !bagComponent.IsCanAddItem(equipItem) )
                {
                    bagComponent.AddItem(bagItem);
                    response.Error = ErrorCode.ERR_AddBagItemError;
                    reply();
                    return;
                }
                equipItem = equipmentsComponent.UnloadEquipItemByPosition(equipPosition);
                bagComponent.AddItem(equipItem);
            }
            
            if (!equipmentsComponent.EquipItem(bagItem))
            {
                response.Error = ErrorCode.ERR_EquipItemError;
                reply();
                return;
            }
            
            reply();
            await ETTask.CompletedTask;
        }
    }
}