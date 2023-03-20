namespace ET
{
    [FriendClass(typeof(AIComponent))]
    public class AI_IDLE: AAIHandler
    {
        public override int Check(AIComponent aiComponent, AIConfig aiConfig)
        {
#if SERVER
            Unit myUnit = aiComponent.GetParent<Unit>();
            Log.Info("Check");
            if (myUnit == null)
            {
                Log.Info("myUnit == null");
                return 1;
            }
            
            BattleUnitFindComponent battleUnitFind = myUnit.GetComponent<BattleUnitFindComponent>();
            if (battleUnitFind == null)
            {
                Log.Info("battleUnitFind == null");
                return 2;
            }

            if (myUnit.Type == UnitType.Player)
            {
                if (battleUnitFind.HasLifeMonsetr() == null)
                {
                    return 0;
                }
            }
            else
            {
                if ( battleUnitFind.HasLifePlayer()== null)
                {
                    Log.Info("No Target");
                    return 0;
                }
            }
            
#endif
            return 0;

        }

        public override async ETTask Execute(AIComponent aiComponent, AIConfig aiConfig, ETCancellationToken cancellationToken)
        {
            await ETTask.CompletedTask;
        }
    }
}