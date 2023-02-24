using System;

namespace ET
{
    public class C2M_AddAttributePointHandler : AMActorLocationRpcHandler<Unit,C2M_AddAttributePoint,M2C_AddAttributePoint>
    {
        protected override async ETTask Run(Unit unit, C2M_AddAttributePoint request, M2C_AddAttributePoint response, Action reply)
        {
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            int targetNumericType = request.NumericType;

           
            // if ( !PlayerNumericConfigCategory.Instance.Contain(targetNumericType) )
            // {
            //     response.Error = ErrorCode.ERR_NumericTypeNotExist;
            //     reply();
            //     return;
            // }
            //
            // PlayerNumericConfig config = PlayerNumericConfigCategory.Instance.Get(targetNumericType);
            // if (config.isAddPoint == 0)
            // {
            //     response.Error = ErrorCode.ERR_NumericTypeNotAddPoint;
            //     reply();
            //     return;
            // }

            int AttributePointCount = numericComponent.GetAsInt(NumericType.AttributePoint);

            if (AttributePointCount <= 0)
            {
                response.Error = ErrorCode.ERR_AddPointNotEnough;
                reply();
                return;
            }

            
            --AttributePointCount;
            numericComponent.Set(NumericType.AttributePoint,AttributePointCount);

            int targetAttributeCount = numericComponent.GetAsInt(targetNumericType);
            if (targetNumericType == NumericType.Atk  || targetNumericType == NumericType.Def)
            {
                targetAttributeCount += 5;
            }
            else if(targetNumericType == NumericType.Hp)
            {
                targetAttributeCount += 20;
            }   
            else if(targetNumericType == NumericType.Dmg)
            {
                targetAttributeCount += 1;
            }

 


            numericComponent.Set(targetNumericType,targetAttributeCount);
            
            //await numericComponent.AddOrUpdateUnitCache();  //关键数据立即存库 

            await ETTask.CompletedTask;
            reply();
        }
    }
}