namespace ET
{
    [FriendClass(typeof(AIComponent))]
    public class AI_ATK: AAIHandler
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

            if (myUnit.Type != UnitType.Player && myUnit.Position != myUnit.OldPosition)
            {
                Log.Info("moving");
                return 2;
            }
            BattleUnitFindComponent battleUnitFind = myUnit.GetComponent<BattleUnitFindComponent>();
            if (battleUnitFind == null)
            {
                Log.Info("battleUnitFind == null");
                return 3;
            }
            if (myUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.IsAlive) == 0)
            {
                Log.Info("No Target");
                return 4;
            }
            if ( battleUnitFind.CheckHasLifePlayer() == false)
            {
                Log.Info("self is die");
                return 5;
            }

            if (myUnit.Type == UnitType.Player)
            {
                if (battleUnitFind.GetNearestMonsetr() != null)
                {
                    return 0;
                }
                else
                {
                    Log.Info("No Target");
                    return 6;
                }
            }
#endif
            return 0;
        }

        public override async ETTask Execute(AIComponent aiComponent, AIConfig aiConfig, ETCancellationToken cancellationToken)
        {

#if SERVER
            Log.Debug("开始攻击");
            Unit myUnit = aiComponent.GetParent<Unit>();
            if (myUnit == null)
            {
                return;
            }
            
            for (int i = 0; i < 100000; ++i)
            {
                BattleUnitFindComponent battleUnitFind = myUnit.GetComponent<BattleUnitFindComponent>();
                Unit target = null;
                if (myUnit.Type ==UnitType.Player)
                {
                    target = battleUnitFind.GetNearestMonsetr();
                    if (target == null)
                    {
                        return;
                    }
                    
                }
                else
                {
                    target = battleUnitFind.GetNearestPlayer().GetResult();
                }
                
                Log.Debug($"攻击: {i}次");
                //计算伤害
                int damage = DamageCalcuateHelper.CalcuateDamageValue(myUnit, target);
                BattleHelper.Damage(myUnit.GetComponent<CombatUnitComponent>(),target.GetComponent<CombatUnitComponent>(),damage);
                // 因为协程可能被中断，任何协程都要传入cancellationToken，判断如果是中断则要返回
                float atkSpeed = myUnit.GetComponent<NumericComponent>().GetAsFloat(NumericType.ATKSpeed)*1000;
                bool timeRet = await TimerComponent.Instance.WaitAsync((long)atkSpeed, cancellationToken);
                if (!timeRet)
                {
                    return;
                }
            }
#endif

            await ETTask.CompletedTask;


        }
    }
}