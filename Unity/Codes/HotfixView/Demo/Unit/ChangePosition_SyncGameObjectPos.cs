using UnityEngine;
using UnityEngine.Rendering;

namespace ET
{
    [FriendClass(typeof(GameObjectComponent))]
    public class ChangePosition_SyncGameObjectPos: AEventClass<EventType.ChangePosition>
    {
        protected override void Run(object changePosition)
        {
            EventType.ChangePosition args = changePosition as EventType.ChangePosition;;
            GameObjectComponent gameObjectComponent = args.Unit.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
            {
                return;
            }
            Transform transform = gameObjectComponent.GameObject.transform;
            transform.position = args.Unit.Position;

            SortingGroup sortingGroup = transform.GetComponent<SortingGroup>();

            if (sortingGroup == null)
            {
                return;
            }
            
            sortingGroup.sortingOrder = (int)-args.Unit.Position.y ;
        }
    }
}