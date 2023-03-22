using System.Collections.Generic;

namespace ET
{

    public static class EquipmentFactory
    {
      
        public static Item Create(Entity parent, int equipmentpos,List<ForgeInfoProto> infoProtos)
        {
            Item item = null;
            int cfgId = 1000 + equipmentpos;
            item = ItemFactory.Create(parent, cfgId);
            EquipInfoComponent infoComponent = item.GetComponent<EquipInfoComponent>();
            for (int i = 0; i < infoProtos.Count; i++)
            {
                int num = infoProtos[i].Num +1;
                
                
                int[] atrs = ForgeAttributeConfigCategory.Instance.Get(infoProtos[i].MarCfgId).Attributes;
                int[] atrRage = ForgeAttributeConfigCategory.Instance.Get(infoProtos[i].MarCfgId).AttributeRage;
                for (int j = 0; j < atrs.Length; j++)
                {
                    int temp = RandomHelper.RandomNumber(0,num);
                    num -= temp;
                    if (temp > 0)
                    {
                        infoComponent.CreateOneEntry(atrs[j],temp * atrRage[j]);
                    }
                }
            }
            return item;
        }
        
    }
}