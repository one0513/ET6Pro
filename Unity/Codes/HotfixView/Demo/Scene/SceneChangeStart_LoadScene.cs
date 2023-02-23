namespace ET
{
    [FriendClass(typeof(SceneLoadComponent))]
    public class SceneChangeStart_LoadScene: AEventAsync<EventType.SceneChangeStart>
    {
        protected override async ETTask Run(EventType.SceneChangeStart args)
        {
            Scene currentScene = args.ZoneScene.CurrentScene();
            
            await UIManagerComponent.Instance.CloseWindow<UILobbyView>();

            await UIManagerComponent.Instance.OpenWindow<UIMainView,Scene>(UIMainView.PrefabPath,args.ZoneScene);
            // SceneLoadComponent slc = EnterMap(currentScene);
            // await AOISceneViewComponent.Instance.ChangeToScene(args.Name,slc);
            // slc.Dispose();
        }

        public SceneLoadComponent EnterMap(Entity self)
        {
            var slc = self.AddComponent<SceneLoadComponent>();
            var role = UnitConfigCategory.Instance.GetAll();
            foreach (var item in role)
                slc.AddPreloadGameObject(item.Value.Perfab, 1);
            //可以走配表
            slc.AddPreloadGameObject("GameAssets/Map/Prefabs/Ground.prefab", 1);
            slc.AddPreloadGameObject("GameAssets/Map/Prefabs/Cube.prefab", 10);
            return slc;
        }
    }
}