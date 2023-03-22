using ET.EventType;

namespace ET.Event
{
    [FriendClass(typeof(AttributeEntry))]
    [FriendClass(typeof(EquipInfoComponent))]
    public class ChangeEquipItemEvent_ChangeNumeric : AEvent<ChangeEquipItem>
    {
        protected override void Run(ChangeEquipItem args)
        {
            EquipInfoComponent equipInfoComponent = args.Item.GetComponent<EquipInfoComponent>();

            if (equipInfoComponent == null)
            {
                return;
            }

            NumericComponent numericComponent = args.Unit.GetComponent<NumericComponent>();
            foreach (var entry in equipInfoComponent.EntryList)
            {
                int numericTypeKey = entry.Key;// * 10 + 2;
                if (args.EquipOp == EquipOp.Load)
                {
                    numericComponent[numericTypeKey] += entry.Value;
                }
                else if (args.EquipOp == EquipOp.Unload)
                {
                    numericComponent[numericTypeKey] -= entry.Value;
                }
            }
        }
    }
}