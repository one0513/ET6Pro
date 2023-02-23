using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class NumericNoticeComponent : Entity,IAwake
    {
        public M2C_NoticeUnitNumeric NoticeUnitNumericMessage = new M2C_NoticeUnitNumeric();
    }
}