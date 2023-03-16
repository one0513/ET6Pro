namespace ET
{
    public static class SceneChangeHelper
    {
        private static WaitType.Wait_CreateMyUnit waitCreateMyUnit { get; set; }

        // 场景切换协程
        public static async ETTask SceneChangeTo(Scene zoneScene, string sceneName, long sceneInstanceId)
        {
            zoneScene.RemoveComponent<AIComponent>();
            
            CurrentScenesComponent currentScenesComponent = zoneScene.GetComponent<CurrentScenesComponent>();
            currentScenesComponent.Scene?.Dispose(); // 删除之前的CurrentScene，创建新的
            Scene currentScene = SceneFactory.CreateCurrentScene(sceneInstanceId, zoneScene.Zone, sceneName, currentScenesComponent);
            UnitComponent unitComponent = currentScene.AddComponent<UnitComponent>();
            currentScene.AddComponent<AdventureComponent>();
         
            // 可以订阅这个事件中创建Loading界面
            //Game.EventSystem.Publish(new EventType.SceneChangeStart() {ZoneScene = zoneScene});
           
            // 等待CreateMyUnit的消息
            WaitType.Wait_CreateMyUnit waitCreateMyUnit = await zoneScene.GetComponent<ObjectWait>().Wait<WaitType.Wait_CreateMyUnit>();
            M2C_CreateMyUnit m2CCreateMyUnit = waitCreateMyUnit.Message;
            Unit myUnit = UnitFactory.CreatePlayer(currentScene, m2CCreateMyUnit.Unit);

            if (myUnit.GetComponent<NumericComponent>().GetAsLong(NumericType.RoomID) != 0)
            {
                WaitType.Wait_InitMonsterInfoList waitInitMonsterInfoList = await zoneScene.GetComponent<ObjectWait>().Wait<WaitType.Wait_InitMonsterInfoList>();
                M2C_InitMonsterInfoList m2CInitMonsterInfoList = waitInitMonsterInfoList.Message;
                UnitFactory.InitMonsterList(currentScene, m2CInitMonsterInfoList.UnitList);
            }
            
            Game.EventSystem.PublishAsync(new EventType.SceneChangeFinish() {ZoneScene = zoneScene, CurrentScene = currentScene}).Coroutine();
            
            // 通知等待场景切换的协程
            zoneScene.GetComponent<ObjectWait>().Notify(new WaitType.Wait_SceneChangeFinish());
        }

        
        
        
        
        
        public static async ETTask WaitChangeScene(Scene zoneScene,string sceneName)
        {
            await Game.EventSystem.PublishAsync(new EventType.SceneChangeStart() { ZoneScene = zoneScene, Name = sceneName });
        }
        
        public static async ETTask WaitCreateMyUnit(Scene zoneScene)
        {
            waitCreateMyUnit = await zoneScene.GetComponent<ObjectWait>().Wait<WaitType.Wait_CreateMyUnit>();
        }
    }
}