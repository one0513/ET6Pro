using System;

namespace ET
{
    [FriendClass(typeof(Item))]
    public class C2M_StartForgeHandler: AMActorLocationRpcHandler<Unit,C2M_StartForge,M2C_StartForge>
    {
        protected override async ETTask Run(Unit unit, C2M_StartForge request, M2C_StartForge response, Action reply)
        {
            await ETTask.CompletedTask;
            BagComponent bag = unit.GetComponent<BagComponent>();
            for (int i = 0; i < request.Infos.Count; i++)
            {
                if (bag.GetItemById(request.Infos[i].ItemUid) == null || bag.GetItemById(request.Infos[i].ItemUid).Count < request.Infos[i].Num)
                {
                    response.Error = ErrorCode.ERR_ForgeMarNotEnough;
                    reply();
                    return;
                }
            }
            //开始锻造
            for (int i = 0; i < request.Infos.Count; i++)
            {
                request.Infos[i].MarCfgId = bag.GetItemById(request.Infos[i].ItemUid).ConfigId;
            }
            
            Item equip = EquipmentFactory.Create(bag, request.ForgePos, request.Infos);
            bag.AddItem(equip);
            
            for (int i = 0; i < request.Infos.Count; i++)
            {
                bag.UseItem(bag.GetItemById(request.Infos[i].ItemUid),request.Infos[i].Num);
            }

            response.ItemInfo = equip.ToMessage();
            reply();


        }
    }
}