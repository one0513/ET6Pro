using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(ZhuiZhuAimComponent))]
    public class AI_ZhuiZhu: AAIHandler
    {
        public override int Check(AIComponent aiComponent, AIConfig aiConfig)
        {
            Unit myUnit = aiComponent.GetParent<Unit>();
            //Log.Info("Check");
            if (myUnit == null)
            {
                Log.Info("myUnit == null");
                return 1;
            }
            ZhuiZhuAimComponent zhuiZhuAimPathComponent = myUnit.GetComponent<ZhuiZhuAimComponent>();
            if (zhuiZhuAimPathComponent == null||zhuiZhuAimPathComponent.Aim==null)
            {
                Log.Info("zhuiZhuAimPathComponent == null||zhuiZhuAimPathComponent.Aim==null");
                return 2;
            }
            
            Vector3 nextTarget = zhuiZhuAimPathComponent.Aim.Position;
            Vector2 tar = new Vector2(nextTarget.x, nextTarget.z);
            Vector2 my = new Vector2(myUnit.Position.x, myUnit.Position.z);
            
            if (Vector2.Distance(tar,my)<2f)
            {
                Log.Info("do not need zhuizhu");
                return 3;
            }
            

            return 0;
        }

        public override async ETTask Execute(AIComponent aiComponent, AIConfig aiConfig, ETCancellationToken cancellationToken)
        {
            Unit myUnit = aiComponent.GetParent<Unit>();
            if (myUnit == null)
            {
                return;
            }
            
            ZhuiZhuAimComponent zhuiZhuAimPathComponent = myUnit.GetComponent<ZhuiZhuAimComponent>();
            
            while (true)
            {
                Vector2 tar = new Vector2(zhuiZhuAimPathComponent.Aim.Position.x, zhuiZhuAimPathComponent.Aim.Position.z);
                Vector2 my = new Vector2(myUnit.Position.x, myUnit.Position.z);
                float dis = Vector2.Distance(tar,my);
                float fix = (dis - 2f) / dis;// 1f 为角色半径 + 怪物攻击范围 后面需要拆开
                float x =  Mathf.Lerp(myUnit.Position.x, zhuiZhuAimPathComponent.Aim.Position.x, fix);
                float z =  Mathf.Lerp(myUnit.Position.z, zhuiZhuAimPathComponent.Aim.Position.z, fix);
                Vector3 nextTarget = new Vector3(x , myUnit.Position.y, z);
#if SERVER //纯客户端单机游戏自己在客户端接入Recast库后去掉

                if (Vector2.Distance(new Vector2(x,z),my)>0.3f)
                {
                    Log.Info("开始追逐");
                    List<Unit> units = new List<Unit>() { zhuiZhuAimPathComponent.Aim };
                    myUnit.MonsterChangePosAsync(nextTarget,units).Coroutine();
                }
              
                await TimerComponent.Instance.WaitAsync(10,cancellationToken);
#else
                myUnit.MoveToAsync(nextTarget, cancellationToken).Coroutine(); 
                await TimerComponent.Instance.WaitAsync(10,cancellationToken);
#endif
               
                if(Vector3.Distance(nextTarget,myUnit.Position)<0.1f) 
                    zhuiZhuAimPathComponent.Arrived();
            }
        }
    }
}