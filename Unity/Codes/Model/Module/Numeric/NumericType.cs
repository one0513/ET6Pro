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
                    __Map.Add("MaxHp",MaxHp);
                    __Map.Add("MaxHpBase",MaxHpBase);
                    __Map.Add("IsAlive",IsAlive);
                    __Map.Add("IsAliveBase",IsAliveBase);
                    __Map.Add("AdventureState",AdventureState);
                    __Map.Add("AdventureStateBase",AdventureStateBase);
                    __Map.Add("DyingState",DyingState);
                    __Map.Add("DyingStateBase",DyingStateBase);
                    __Map.Add("AdventureStartTime",AdventureStartTime);
                    __Map.Add("AdventureStartTimeBase",AdventureStartTimeBase);
                    __Map.Add("CurLevel",CurLevel);
                    __Map.Add("CurLevelBase",CurLevelBase);
                    __Map.Add("Speed",Speed);
                    __Map.Add("SpeedBase",SpeedBase);
                    __Map.Add("PosX",PosX);
                    __Map.Add("PosXBase",PosXBase);
                    __Map.Add("PosZ",PosZ);
                    __Map.Add("PosZBase",PosZBase);
                    __Map.Add("AOI",AOI);
                    __Map.Add("AOIBase",AOIBase);
                    __Map.Add("RoomID",RoomID);
                    __Map.Add("RoomIDBase",RoomIDBase);
                    __Map.Add("ATKSpeed",ATKSpeed);
                    __Map.Add("ATKSpeedBase",ATKSpeedBase);
                    __Map.Add("ATKRage",ATKRage);
                    __Map.Add("ATKRageBase",ATKRageBase);
                    __Map.Add("RestartTime",RestartTime);
                    __Map.Add("RestartTimeBase",RestartTimeBase);
                    __Map.Add("MaxBagCapacity",MaxBagCapacity);
                    __Map.Add("MaxBagCapacityBase",MaxBagCapacityBase);
                    __Map.Add("Gold",Gold);
                    __Map.Add("GoldBase",GoldBase);
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

		/// <summary> 最大生命 </summary>
		public const int MaxHp = 3010;
		/// <summary> 最大生命Base </summary>
		public const int MaxHpBase = 3010 * 10 + 1;

		/// <summary> 是否存活 </summary>
		public const int IsAlive = 3011;
		/// <summary> 是否存活Base </summary>
		public const int IsAliveBase = 3011 * 10 + 1;

		/// <summary> 冒险状态 </summary>
		public const int AdventureState = 3012;
		/// <summary> 冒险状态Base </summary>
		public const int AdventureStateBase = 3012 * 10 + 1;

		/// <summary> 垂死状态 </summary>
		public const int DyingState = 3013;
		/// <summary> 垂死状态Base </summary>
		public const int DyingStateBase = 3013 * 10 + 1;

		/// <summary> 战斗开始时间 </summary>
		public const int AdventureStartTime = 3014;
		/// <summary> 战斗开始时间Base </summary>
		public const int AdventureStartTimeBase = 3014 * 10 + 1;

		/// <summary> 当前关卡 </summary>
		public const int CurLevel = 3015;
		/// <summary> 当前关卡Base </summary>
		public const int CurLevelBase = 3015 * 10 + 1;

		/// <summary> 移动速度 </summary>
		public const int Speed = 3016;
		/// <summary> 移动速度Base </summary>
		public const int SpeedBase = 3016 * 10 + 1;

		/// <summary> 坐标X </summary>
		public const int PosX = 3017;
		/// <summary> 坐标XBase </summary>
		public const int PosXBase = 3017 * 10 + 1;

		/// <summary> 坐标Z </summary>
		public const int PosZ = 3018;
		/// <summary> 坐标ZBase </summary>
		public const int PosZBase = 3018 * 10 + 1;

		/// <summary> 视野 </summary>
		public const int AOI = 3019;
		/// <summary> 视野Base </summary>
		public const int AOIBase = 3019 * 10 + 1;

		/// <summary> 小队ID </summary>
		public const int RoomID = 3020;
		/// <summary> 小队IDBase </summary>
		public const int RoomIDBase = 3020 * 10 + 1;

		/// <summary> 攻击速度 </summary>
		public const int ATKSpeed = 3021;
		/// <summary> 攻击速度Base </summary>
		public const int ATKSpeedBase = 3021 * 10 + 1;

		/// <summary> 攻击范围 </summary>
		public const int ATKRage = 3022;
		/// <summary> 攻击范围Base </summary>
		public const int ATKRageBase = 3022 * 10 + 1;

		/// <summary> 复活时间 </summary>
		public const int RestartTime = 3023;
		/// <summary> 复活时间Base </summary>
		public const int RestartTimeBase = 3023 * 10 + 1;

		/// <summary> 背包最大容量 </summary>
		public const int MaxBagCapacity = 3024;
		/// <summary> 背包最大容量Base </summary>
		public const int MaxBagCapacityBase = 3024 * 10 + 1;

		/// <summary> 金币 </summary>
		public const int Gold = 3025;
		/// <summary> 金币Base </summary>
		public const int GoldBase = 3025 * 10 + 1;
    }
}
