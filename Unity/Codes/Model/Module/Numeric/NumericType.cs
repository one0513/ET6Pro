using System.Collections.Generic;
namespace ET
{
    public class NumericType
    {
        public int this[string key]
        {
            get
            {
                if (Map.TryGetValue(key, out var res))
                {
                    return res;
                }
                Log.Error($"{key}属性不存在");
                return -1;
            }
        }
        private static Dictionary<string, int> __Map;
        public static Dictionary<string, int> Map
        {
            get
            {
                if (__Map == null)
                {
                    __Map = new Dictionary<string, int>();
                    __Map.Add("Lv",Lv);
                    __Map.Add("LvBase",LvBase);
                    __Map.Add("Exp",Exp);
                    __Map.Add("ExpBase",ExpBase);
                    __Map.Add("Atk",Atk);
                    __Map.Add("AtkBase",AtkBase);
                    __Map.Add("Def",Def);
                    __Map.Add("DefBase",DefBase);
                    __Map.Add("Hp",Hp);
                    __Map.Add("HpBase",HpBase);
                    __Map.Add("Dmg",Dmg);
                    __Map.Add("DmgBase",DmgBase);
                    __Map.Add("BattleRandomSeed",BattleRandomSeed);
                    __Map.Add("BattleRandomSeedBase",BattleRandomSeedBase);
                    __Map.Add("AttributePoint",AttributePoint);
                    __Map.Add("AttributePointBase",AttributePointBase);
                    __Map.Add("CE",CE);
                    __Map.Add("CEBase",CEBase);
                }
                return __Map;
            }
        }
		public const int Max = 10000;

		/// <summary> 等级 </summary>
		public const int Lv = 3001;
		/// <summary> 等级Base </summary>
		public const int LvBase = 3001 * 10 + 1;

		/// <summary> 经验 </summary>
		public const int Exp = 3002;
		/// <summary> 经验Base </summary>
		public const int ExpBase = 3002 * 10 + 1;

		/// <summary> 攻击 </summary>
		public const int Atk = 3003;
		/// <summary> 攻击Base </summary>
		public const int AtkBase = 3003 * 10 + 1;

		/// <summary> 防御 </summary>
		public const int Def = 3004;
		/// <summary> 防御Base </summary>
		public const int DefBase = 3004 * 10 + 1;

		/// <summary> 生命 </summary>
		public const int Hp = 3005;
		/// <summary> 生命Base </summary>
		public const int HpBase = 3005 * 10 + 1;

		/// <summary> 伤害 </summary>
		public const int Dmg = 3006;
		/// <summary> 伤害Base </summary>
		public const int DmgBase = 3006 * 10 + 1;

		/// <summary> 战斗随机数 </summary>
		public const int BattleRandomSeed = 3007;
		/// <summary> 战斗随机数Base </summary>
		public const int BattleRandomSeedBase = 3007 * 10 + 1;

		/// <summary> 属性点 </summary>
		public const int AttributePoint = 3008;
		/// <summary> 属性点Base </summary>
		public const int AttributePointBase = 3008 * 10 + 1;

		/// <summary> 战斗力 </summary>
		public const int CE = 3009;
		/// <summary> 战斗力Base </summary>
		public const int CEBase = 3009 * 10 + 1;
    }
}
