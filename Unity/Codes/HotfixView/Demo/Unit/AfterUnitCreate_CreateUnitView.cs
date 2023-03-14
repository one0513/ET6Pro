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
            GameObject bundleGameObject = AssetsBundleHelper.LoadOneAssetSync(args.Unit.Config.PrefabName) as GameObject;
            GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
            go.transform.SetParent(GlobalComponent.Instance.Unit,true);
            go.transform.localScale = new Vector3(1, 1, 1);
            args.Unit.AddComponent<GameObjectComponent>().GameObject = go;
            args.Unit.GetComponent<GameObjectComponent>().SpriteRenderer = go.GetComponent<SpriteRenderer>();
            args.Unit.AddComponent<AnimatorComponent>();
            args.Unit.AddComponent<HeadHpViewComponent>();
            if (args.Unit.Type == UnitType.Player)
            {
                float x =  args.Unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.PosX);
                float z =  args.Unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.PosZ);
                args.Unit.Position = new Vector3(x, 0, z);
            }
            else
            {
                args.Unit.Position = args.Unit.Position;
            }
            
    
            
            //args.Unit.Position = args.Unit.Type == UnitType.Player? new Vector3(-1.5f, 0, 0) : new Vector3(1.5f,RandomHelper.RandomNumber(-1,1) , 0);
            
            await ETTask.CompletedTask;
        }
    }
}