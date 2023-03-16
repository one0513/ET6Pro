using System;
using UnityEngine;
using System.Collections.Generic;

namespace ET
{
    public static class UnitFactory
    {

        public static void AfterCreateUnitFromMsg(Unit unit,CreateUnitFromMsgType type)
        {
            var scene = unit.Parent.GetParent<Scene>();
            UnitType unitType = unit.Type;
            switch (unitType)
            {
                case UnitType.Player:
                {
                    if (unit.GetComponent<AOIUnitComponent>()==null)
                    {
                        unit.AddComponent<MoveComponent>();
                        MapSceneConfig conf = MapSceneConfigCategory.Instance.Get((int) scene.Id);
                        unit.AddComponent<PathfindingComponent, string>(conf.Recast);
                        var numericComponent = unit.GetComponent<NumericComponent>();

                        // 加入aoi
                        // var aoiu = unit.AddComponent<AOIUnitComponent, Vector3, Quaternion, UnitType, int,bool>(unit.Position, unit.Rotation, unit.Type,
                        //     numericComponent.GetAsInt(NumericType.AOI),type!=CreateUnitFromMsgType.Create);
                        // aoiu.AddSphereCollider(0.5f);
                        // if (type != CreateUnitFromMsgType.Create)
                        // {
                        //     aoiu.GetComponent<GhostComponent>().IsGoast = type == CreateUnitFromMsgType.Add;
                        // }
                    }
                    else
                    {
                        var aoiu = unit.GetComponent<AOIUnitComponent>();
                        aoiu.GetComponent<GhostComponent>().IsGoast = type == CreateUnitFromMsgType.Add;
                    }
                    if (type!=CreateUnitFromMsgType.Add)
                    {
                        if (unit.GetComponent<MailBoxComponent>() == null)
                        {
                            unit.AddComponent<MailBoxComponent>();
                        }
                        else
                        {
                            Log.Error("??? unit.GetComponent<MailBoxComponent>() != null");
                        }
                    }
                    break;
                }
                case UnitType.Skill:
                {
                    if (unit.GetComponent<AOIUnitComponent>()==null)
                    {
                        var skillInfo = unit.GetComponent<SkillColliderComponent>();
                        var pos = unit.Position;
                        var collider = skillInfo.Config;
                        if (collider.ColliderType == ColliderType.Target)//朝指定位置方向飞行碰撞体
                        {
                            var moveComp = unit.AddComponent<MoveComponent>();
                            List<Vector3> target = new List<Vector3>();
                            target.Add(pos);
                            target.Add(pos + (skillInfo.Position - pos).normalized * collider.Speed * collider.Time / 1000f);
                            moveComp.MoveToAsync(target, collider.Speed).Coroutine();
                        }
                        else if (collider.ColliderType == ColliderType.Aim) //锁定目标飞行
                        {
                            var toUnit = unit.Parent.GetChild<Unit>(skillInfo.ToId);
                            unit.AddComponent<ZhuiZhuAimComponent, Unit, Action>(toUnit, () =>
                            {
                                unit.Dispose();
                            });
                            unit.AddComponent<AIComponent,int,int>(2,50);
                        }
                        var aoiu =unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType,bool>(pos,unit.Rotation,unit.Type,type!=CreateUnitFromMsgType.Create);
                        skillInfo.OnCreate();
                        if (type != CreateUnitFromMsgType.Create)
                        {
                            aoiu.GetComponent<GhostComponent>().IsGoast = type == CreateUnitFromMsgType.Add;
                        }
                    }
                    else
                    {
                        var aoiu = unit.GetComponent<AOIUnitComponent>();
                        aoiu.GetComponent<GhostComponent>().IsGoast = type == CreateUnitFromMsgType.Add;
                    }
                    break;
                }

            }
            
        }

        public static Unit Create(Scene scene, long id, UnitType unitType)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            switch (unitType)
            {
                case UnitType.Player:
                {
                    Unit unit = unitComponent.AddChildWithId<Unit, int>(id, 1001);
                    NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
                    foreach (var config in PlayerNumericConfigCategory.Instance.GetAll())
                    {
                        if ( config.Value.BaseValue == 0 )
                        {
                            continue;
                        }

                        if ( config.Key < 3000 ) //小于3000的值都用加成属性推导
                        {
                            int baseKey = config.Key * 10 + 1;
                            numericComponent.SetNoEvent(baseKey,config.Value.BaseValue);
                        }
                        else
                        {
                            //大于3000的值 直接使用
                            numericComponent.SetNoEvent(config.Key,config.Value.BaseValue);
                        }
                    }
                    unit.AddComponent<TasksComponent>();
                    unitComponent.Add(unit);
                    // 进入地图再加入aoi
                    
                    return unit;
                }
                default:
                    throw new Exception($"not such unit type: {unitType}");
            }
        }
        
        public static Unit CreateSkillCollider(Scene currentScene, int configId, Vector3 pos,Quaternion rota,SkillPara para)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChild<Unit,int>(configId);
        
            unit.Position = pos;
            unit.Rotation = rota;
            var collider = SkillJudgeConfigCategory.Instance.Get(configId);
            if (collider.ColliderType == ColliderType.Target)//朝指定位置方向飞行碰撞体
            {
                var numc = unit.AddComponent<NumericComponent>();
                //numc.Set(NumericType.SpeedBase, collider.Speed);
                var moveComp = unit.AddComponent<MoveComponent>();
                List<Vector3> target = new List<Vector3>();
                target.Add(pos);
                target.Add(pos + (para.Position - pos).normalized * collider.Speed * collider.Time / 1000f);
                moveComp.MoveToAsync(target, collider.Speed).Coroutine();
                unit.AddComponent<SkillColliderComponent,SkillPara,Vector3>(para,para.Position);
            }
            else if (collider.ColliderType == ColliderType.Aim) //锁定目标飞行
            {
                var numc = unit.AddComponent<NumericComponent>();
                //numc.Set(NumericType.SpeedBase,collider.Speed);
                unit.AddComponent<MoveComponent>();
                unit.AddComponent<ZhuiZhuAimComponent, Unit, Action>(para.To.unit, () =>
                {
                    unit.Dispose();
                });
                unit.AddComponent<AIComponent,int,int>(2,50);
                unit.AddComponent<SkillColliderComponent,SkillPara,long>(para,para.To.Id);
            }
            else
            {
                unit.AddComponent<SkillColliderComponent, SkillPara>(para);
            }
            unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType>(pos,rota,unit.Type);
            return unit;
        }
        
        public static Unit CreateMonster(Scene scene,long roomId ,int configId)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.Instance.GenerateId(), configId);
            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            
            numericComponent.SetNoEvent(NumericType.MaxHp,unit.Config.MaxHP);
            numericComponent.SetNoEvent(NumericType.Hp,unit.Config.MaxHP);
            numericComponent.SetNoEvent(NumericType.Dmg,unit.Config.Dmg);
            numericComponent.SetNoEvent(NumericType.Atk,unit.Config.Atk);
            numericComponent.SetNoEvent(NumericType.Def,unit.Config.Def);
            numericComponent.SetNoEvent(NumericType.IsAlive,1);
            numericComponent.SetNoEvent(NumericType.RoomID,roomId);
            
            unitComponent.Add(unit);

            unit.AddComponent<MoveComponent>();
            unit.AddComponent<PathfindingComponent, string>("Map");
            
            unit.AddComponent<AIComponent,int>(3);
            unit.AddComponent<CombatUnitComponent>();
            // var aoiu = unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType,int>
            //         (unit.Position,unit.Rotation,unit.Type,5);
			         //
            // aoiu.AddSphereCollider(0.5f);
            return unit;
        }
    }
}