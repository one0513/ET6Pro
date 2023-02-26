using System;

namespace ET
{
    public static class DamageCalcuateHelper
    {
        public static int CalcuateDamageValue(Unit attackUnit,Unit TargetUnit,ref SRandom random)
        {
            int dmg = attackUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.Dmg);
            int atk = attackUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.Atk);
            int targetDef  = TargetUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.Def);

            float alldamage = (float)atk / (float)targetDef * dmg;
            int damage = (int)Math.Ceiling(alldamage);
            
            Log.Debug($"造成伤害值：{damage}");
            return damage;
        }
    }
}