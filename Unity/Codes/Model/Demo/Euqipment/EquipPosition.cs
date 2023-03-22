namespace ET
{
    /// <summary>
    /// 装备装配部位
    /// </summary>
    public enum EquipPosition
    {
        None    = 0, //不可装备
        Weapon    = 1, //武器
        Deputy = 2, //副手
        Casque   = 3, //头盔
        Armour    = 4, //盔甲

    }
    
    /// <summary>
    /// 装备操作指令
    /// </summary>
    public enum EquipOp
    {
        Load,   //穿戴
        Unload, //卸下
    }
    
    
}