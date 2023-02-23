using UnityEngine;

namespace ET
{
    [FriendClass(typeof(GlobalComponent))]
    [FriendClass(typeof(GameObjectComponent))]
    public class AfterUnitCreate_CreateUnitView: AEventAsync<EventType.AfterUnitCreate>
    {
        protected override async ETTask Run(EventType.AfterUnitCreate args)
        {
            // Unit View层
            GameObject bundleGameObject = (GameObject)await AssetsBundleHelper.LoadOneAssetAsync("Unit/Knight Files/Prefabs/Knight.prefab");
            GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
            
            go.transform.SetParent(GlobalComponent.Instance.Unit,true);
            args.Unit.AddComponent<GameObjectComponent>().GameObject = go;
            args.Unit.GetComponent<GameObjectComponent>().SpriteRenderer = go.GetComponent<SpriteRenderer>();
            args.Unit.AddComponent<AnimatorComponent>();

            args.Unit.Position = args.Unit.Type == UnitType.Player? new Vector3(-1.5f, 0, 0) : new Vector3(1.5f,RandomHelper.RandomNumber(-1,1) , 0);
            
            await ETTask.CompletedTask;
        }
    }
}