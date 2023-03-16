using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(RoomInfo))]
    public static class MoveHelper
    {
        // 可以多次调用，多次调用的话会取消上一次的协程
        public static async ETTask FindPathMoveToAsync(this Unit unit, Vector3 target, ETCancellationToken cancellationToken = null)
        {
            float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
            //float speed = 2;
            if (speed < 0.01)
            {
                unit.SendStop(2);
                return;
            }
            
            using var list = ListComponent<Vector3>.Create();
            
            unit.GetComponent<PathfindingComponent>().Find(unit.Position, target, list);
            
            List<Vector3> path = list;
            if (path.Count < 2)
            {
                unit.SendStop(3);
                return;
            }
                
            // 广播寻路路径
            M2C_PathfindingResult m2CPathfindingResult = new M2C_PathfindingResult();
            m2CPathfindingResult.X = unit.Position.x;
            m2CPathfindingResult.Y = unit.Position.y;
            m2CPathfindingResult.Z = unit.Position.z;
            m2CPathfindingResult.Id = unit.Id;
            for (int i = 0; i < list.Count; ++i)
            {
                Vector3 vector3 = list[i];
                m2CPathfindingResult.Xs.Add(vector3.x);
                m2CPathfindingResult.Ys.Add(vector3.y);
                m2CPathfindingResult.Zs.Add(vector3.z);
            }

            await MessageHelper.RoomBroadcast(unit, m2CPathfindingResult);
            //MessageHelper.Broadcast(unit, m2CPathfindingResult);
            
            bool ret = await unit.GetComponent<MoveComponent>().MoveToAsync(path, speed);
            if (ret) // 如果返回false，说明被其它移动取消了，这时候不需要通知客户端stop
            {
                unit.SendStop(0);
            }
            unit.GetComponent<NumericComponent>().Set(NumericType.PosX,target.x);
            unit.GetComponent<NumericComponent>().Set(NumericType.PosZ,target.z);
            await Task.CompletedTask;
        }

        public static async ETTask MoveToAsync(this Unit unit, List<Vector3> path, ETCancellationToken cancellationToken = null)
        {
            //float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
            float speed = 2;
            if (speed < 0.01)
            {
                unit.SendStop(-1);
                return;
            }
            // 广播寻路路径
            M2C_PathfindingResult m2CPathfindingResult = new M2C_PathfindingResult();
            m2CPathfindingResult.X = unit.Position.x;
            m2CPathfindingResult.Y = unit.Position.y;
            m2CPathfindingResult.Z = unit.Position.z;
            m2CPathfindingResult.Id = unit.Id;
            for (int i = 0; i < path.Count; ++i)
            {
                Vector3 vector3 = path[i];
                m2CPathfindingResult.Xs.Add(vector3.x);
                m2CPathfindingResult.Ys.Add(vector3.y);
                m2CPathfindingResult.Zs.Add(vector3.z);
            }
            MessageHelper.Broadcast(unit, m2CPathfindingResult);
            
            bool ret = await unit.GetComponent<MoveComponent>().MoveToAsync(path, speed);
            if (ret) // 如果返回false，说明被其它移动取消了，这时候不需要通知客户端stop
            {
                unit.GetComponent<NumericComponent>().Set(NumericType.PosX,m2CPathfindingResult.Xs[m2CPathfindingResult.Xs.Count-1]);
                unit.GetComponent<NumericComponent>().Set(NumericType.PosZ,m2CPathfindingResult.Zs[m2CPathfindingResult.Zs.Count-1]);
                unit.SendStop(0);
            }
            await Task.CompletedTask;
        }
        
        public static async ETTask MonsterChangePosAsync(this Unit unit, Vector3 target,List<Unit> sendUnits)
        {
            using var list = ListComponent<Vector3>.Create();
            
            unit.GetComponent<PathfindingComponent>().Find(unit.Position, target, list);
            
            List<Vector3> path = list;
            if (path.Count < 2)
            {
                //unit.SendStop(3);
                return;
            }
            
            float speed = 1.5f;
            if (speed < 0.01)
            {
                unit.SendStop(-1);
                return;
            }
            // 广播寻路路径
            M2C_PathfindingResult m2CPathfindingResult = new M2C_PathfindingResult();
            m2CPathfindingResult.X = unit.Position.x;
            m2CPathfindingResult.Y = unit.Position.y;
            m2CPathfindingResult.Z = unit.Position.z;
            m2CPathfindingResult.Id = unit.Id;
            for (int i = 0; i < path.Count; ++i)
            {
                Vector3 vector3 = path[i];
                m2CPathfindingResult.Xs.Add(vector3.x);
                m2CPathfindingResult.Ys.Add(vector3.y);
                m2CPathfindingResult.Zs.Add(vector3.z);
            }
            
            long roomId = unit.GetComponent<NumericComponent>().GetAsLong(NumericType.RoomID);
            RoomInfo info = unit.DomainScene().GetComponent<RoomInfoComponent>().Get(roomId);
            if (info != null)
            {
                for (int i = 0; i < info.playerList.Count; i++)
                {
                    Unit onlineUnit = unit.DomainScene().GetComponent<UnitComponent>().Get(info.playerList[i]);
                    if (onlineUnit != null)
                    {
                        await MessageHelper.RoomBroadcast(onlineUnit, m2CPathfindingResult);
                        break;
                    }
                }
            }
            else
            {
                var infos = await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoomInfo>(d => d.RoomId == roomId);
                for (int i = 0; i < infos[0].playerList.Count; i++)
                {
                    Unit onlineUnit = unit.DomainScene().GetComponent<UnitComponent>().Get(infos[0].playerList[i]);
                    if (onlineUnit != null)
                    {
                        await MessageHelper.RoomBroadcast(onlineUnit, m2CPathfindingResult);
                        break;
                    }
                }
            }
  
            
            bool ret = await unit.GetComponent<MoveComponent>().MoveToAsync(path, speed);
            if (ret) // 如果返回false，说明被其它移动取消了，这时候不需要通知客户端stop
            {
                unit.SendStop(0);
            }
            await Task.CompletedTask;
        }
        
        public static void Stop(this Unit unit, int error)
        {
            unit.GetComponent<MoveComponent>().Stop(error == 0);
            unit.SendStop(error);
        }
        // error: 0表示协程走完正常停止
        public static void SendStop(this Unit unit, int error)
        {
            MessageHelper.Broadcast(unit, new M2C_Stop()
            {
                Error = error,
                Id = unit.Id, 
                X = unit.Position.x,
                Y = unit.Position.y,
                Z = unit.Position.z,
						
                A = unit.Rotation.x,
                B = unit.Rotation.y,
                C = unit.Rotation.z,
                W = unit.Rotation.w,
            });
        }
    }
}