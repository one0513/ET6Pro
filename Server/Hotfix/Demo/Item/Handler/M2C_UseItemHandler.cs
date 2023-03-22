using System;

namespace ET.Handler
{
    public class C2M_UseItemHandler : AMActorLocationRpcHandler<Unit,C2M_UseItem,M2C_UseItem>
    {
        protected override async ETTask Run(Unit unit, C2M_UseItem request, M2C_UseItem response, Action reply)
        {
            await ETTask.CompletedTask;
            Item item = unit.GetComponent<BagComponent>().GetItemById(request.ItemUid);
            if (item == null)
            {
                response.Error = ErrorCode.ERR_ItemNotExist;
                reply();
                return;
            }
            if (item.Config.CanUse != 0)
            {
                response.Error = ErrorCode.ERR_ItemCanNotUse;
                reply();
                return;
            }
            if (item.Count < request.Num)
            {
                response.Error = ErrorCode.ERR_ItemNumNotEnough;
                reply();
                return;
            }

            if (item.Config.UseGain != null)
            {
                unit.GetComponent<NumericComponent>()[item.Config.UseGain[0]] += item.Config.UseGain[1]*request.Num;
                unit.GetComponent<BagComponent>().UseItem(item,request.Num);
            }
            
            reply();
        }
    }
}