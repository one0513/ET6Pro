namespace ET
{
    public static class UnitNumericHelper
    {
        public static bool IsAlive(this Unit unit)
        {
            if (unit == null || unit.IsDisposed)
            {
                return false;
            }

            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            if (null == numericComponent)
            {
                return false;
            }
            
            return numericComponent.GetAsInt(NumericType.IsAlive) == 1;
        }
        
        public static void SetAlive(this Unit unit, bool isAlive)
        {
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            if (null == numericComponent)
            {
                return ;
            }
            
            numericComponent.Set(NumericType.IsAlive,isAlive?1:0);
        }
    }
}