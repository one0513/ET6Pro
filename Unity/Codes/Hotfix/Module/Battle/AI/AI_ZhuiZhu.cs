using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(ZhuiZhuAimComponent))]
    public class AI_ZhuiZhu: AAIHandler
    {
        public override int Check(AIComponent aiComponent, AIConfig aiConfig)
        {
#if SERVER
             Unit myUnit = aiComponent.GetParent<Unit>();
            //Log.Info("Check");
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
            if (battleUnitFind.CheckHasLifePlayer() == false)
            {
                Log.Info("No Target");
                return 3;
            }

            Unit target = battleUnitFind.GetNearestPlayer().GetResult();
            Vector3 nextTarget = target.Position;
            Vector2 tar = new Vector2(nextTarget.x, nextTarget.z);
            Vector2 my = new Vector2(myUnit.Position.x, myUnit.Position.z);

            float dis = Vector2.Distance(tar, my);
            float rage = myUnit.GetComponent<NumericComponent>().GetAsFloat(NumericType.ATKRage);
            rage = rage + 1f + 0.11f;// 玩家半径 + 角色攻击范围 + 0.11f（容差）
            if (dis<rage)
            {
                Log.Info("do not need zhuizhu");
                return 4;
            }
#endif
           
            
            return 0;
        }

        public override async ETTask Execute(AIComponent aiComponent, AIConfig aiConfig, ETCancellationToken cancellationToken)
        {
#if SERVER //纯客户端单机游戏自己在客户端接入Recast库后去掉
            Unit myUnit = aiComponent.GetParent<Unit>();
            if (myUnit == null)
            {
                return;
            }
            
            for (int i = 0; i < 100; ++i)
            {
                BattleUnitFindComponent battleUnitFind = myUnit.GetComponent<BattleUnitFindComponent>();
                Unit target = battleUnitFind.GetNearestPlayer().GetResult();
                if (target!=null)
                {
                    Vector2 tar = new Vector2(target.Position.x, target.Position.z);
                    Vector2 my = new Vector2(myUnit.Position.x, myUnit.Position.z);
                    float dis = Vector2.Distance(tar,my);
                    float rage = myUnit.GetComponent<NumericComponent>().GetAsFloat(NumericType.ATKRage);
                    rage = rage + 1f;// 玩家半径 + 角色攻击范围 1f 为角色半径
                    float fix = (dis - rage) / dis;
                    float x =  Mathf.Lerp(myUnit.Position.x, target.Position.x, fix);
                    float z =  Mathf.Lerp(myUnit.Position.z, target.Position.z, fix);
                    Vector3 nextTarget = new Vector3(x , myUnit.Position.y, z);
                    
                    if (Vector2.Distance(new Vector2(x,z),my)>0.1f)
                    {
                        Log.Info($"开始第{i}次追逐");
                        await myUnit.MonsterChangePosAsync(nextTarget);
                        
                    }
                    
                    bool timeRet = await TimerComponent.Instance.WaitAsync(100, cancellationToken);
                    if (!timeRet)
                    {
                        return;
                    }
                }
            }
            
#else
            await ETTask.CompletedTask;
#endif
        }
    }
}