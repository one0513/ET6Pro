using System.Collections.Generic;
namespace ET
{
    public class NumericType
    {
		public const int Max = 10000;

	    public const int Speed = 1000;
	    public const int SpeedBase = Speed * 10 + 1; 
	    public const int SpeedAdd = Speed * 10 + 2;
	    public const int SpeedPct = Speed * 10 + 3;
	    public const int SpeedFinalAdd = Speed * 10 + 4;
	    public const int SpeedFinalPct = Speed * 10 + 5;
	    
	    public const int MaxHp = 1002;
	    public const int MaxHpBase = MaxHp * 10 + 1;
	    public const int MaxHpAdd = MaxHp * 10 + 2;
	    public const int MaxHpPct = MaxHp * 10 + 3;
	    public const int MaxHpFinalAdd = MaxHp * 10 + 4;
	    public const int MaxHpFinalPct = MaxHp * 10 + 5;

	    public const int AOI = 1003;
	    public const int AOIBase = AOI * 10 + 1;
	    public const int AOIAdd = AOI * 10 + 2;
	    public const int AOIPct = AOI * 10 + 3;
	    public const int AOIFinalAdd = AOI * 10 + 4;
	    public const int AOIFinalPct = AOI * 10 + 5;
	    
	    public const int MaxMp = 1004;
	    public const int MaxMpBase = MaxMp * 10 + 1;
	    public const int MaxMpAdd = MaxMp * 10 + 2;
	    public const int MaxMpPct = MaxMp * 10 + 3;
	    public const int MaxMpFinalAdd = MaxMp * 10 + 4;
	    public const int MaxMpFinalPct = MaxMp * 10 + 5;

	    
	    public const int DamageValue = 1011;         //伤害
	    public const int DamageValueBase = DamageValue * 10 + 1;
	    public const int DamageValueAdd = DamageValue * 10 + 2;
	    public const int DamageValuePct = DamageValue * 10 + 3;
	    public const int DamageValueFinalAdd = DamageValue * 10 + 4;
	    public const int DamageValueFinalPct = DamageValue * 10 + 5;
	    
	    public const int AdditionalDdamage = 1012;         //伤害追加

	    
	    public const int Hp = 1013;  // 生命值
	    public const int HpBase = Hp * 10 + 1;
	    public const int HpAdd = Hp * 10 + 2;
	    public const int HpPct = Hp * 10 + 3;
	    public const int HpFinalAdd = Hp * 10 + 4;
	    public const int HpFinalPct = Hp * 10 + 5;
	    

	    
	    public const int MP = 1014; //法力值
	    public const int MPBase = MP * 10 + 1;
	    public const int MPAdd = MP * 10 + 2;
	    public const int MPPct = MP * 10 + 3;
	    public const int MPFinalAdd = MP * 10 + 4;
	    public const int MPFinalPct = MP * 10 + 5;
	    

	    public const int Armor = 1015; //护甲
	    public const int ArmorBase = Armor * 10 + 1;
	    public const int ArmorAdd = Armor * 10 + 2;
	    public const int ArmorPct = Armor * 10 + 3;
	    public const int ArmorFinalAdd = Armor * 10 + 4;
	    public const int ArmorFinalPct = Armor * 10 + 5;
	    
	    public const int ArmorAddition = 1015; //护甲追加
	    
	    public const int Dodge = 1017;           //闪避
	    public const int DodgeBase = Dodge * 10 + 1;
	    public const int DodgeAdd = Dodge * 10 + 2;
	    public const int DodgePct = Dodge * 10 + 3;
	    public const int DodgeFinalAdd = Dodge * 10 + 4;
	    public const int DodgeFinalPct = Dodge * 10 + 5;

	    public const int DodgeAddition = 1018;   // 闪避追加
	    
	    public const int CriticalHitRate = 1019; //暴击率
	    public const int CriticalHitRateBase = CriticalHitRate * 10 + 1;
	    public const int CriticalHitRateAdd = CriticalHitRate * 10 + 2;
	    public const int CriticalHitRatePct = CriticalHitRate * 10 + 3;
	    public const int CriticalHitRateFinalAdd = CriticalHitRate * 10 + 4;
	    public const int CriticalHitRateFinalPct = CriticalHitRate * 10 + 5;
	    
	    public const int Power = 3001; //力量
	    
	    public const int PhysicalStrength = 3002; //体力

	    public const int Agile = 3003; //敏捷值

	    public const int Spirit = 3004; //精神
	    
	    public const int AttributePoint = 3005; //属性点
	    
	    public const int CombatEffectiveness = 3006; //战力值
	    
	    public const int Level = 3007;
	    
	    public const int Gold  = 3008;
	    
	    public const int Exp   = 3009;

	    public const int AdventureState = 3010;   //关卡冒险状态
	    
	    public const int DyingState     = 3011;      //垂死状态
	    
	    public const int AdventureStartTime = 3012;   //关卡开始冒险的时间

	    public const int IsAlive = 3013;    //存活状态  0为死亡 1为活着


	    public const int BattleRandomSeed = 3014;    //战斗随机数种子
	    
	    public const int MaxBagCapacity  = 3015;   //背包最大负重


	    public const int IronStone = 3016; //铁矿石

	    public const int Fur       = 3017; //皮毛
    }
}
