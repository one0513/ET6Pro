using DG.Tweening;
using TMPro;
using UnityEditor.UI;
using UnityEngine;

namespace ET
{
    public class FlyDamageValueViewComponentAwakeSystem : AwakeSystem<FlyDamageValueViewComponent>
    {
        public override void Awake(FlyDamageValueViewComponent self)
        {
            self.Awake().Coroutine();
        }
    }
    
    
    public class FlyDamageValueViewComponentDestroySystem : DestroySystem<FlyDamageValueViewComponent>
    {
        public override void Destroy(FlyDamageValueViewComponent self)
        {
            ForeachHelper.Foreach<GameObject>(self.FlyingDamageSet, (o) =>
            {
                o.transform.DOKill();
                GameObject.Destroy(o);
            });
            self.FlyingDamageSet.Clear();
        }
    }

    
    [FriendClass(typeof(GlobalComponent))]
    [FriendClass(typeof(FlyDamageValueViewComponent))]
    public static class FlyDamageValueViewComponentSystem
    {
        public static async ETTask Awake(this FlyDamageValueViewComponent self)
        {
            GameObject bundleGameObject =  AssetsBundleHelper.LoadOneAssetSync("flyDamageValue") as GameObject;
            
            //GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
            
            await GameObjectPoolHelper.InitPoolFormGamObjectAsync(bundleGameObject ,3);
        }
        
        
        public static async ETTask SpawnFlyDamage(this FlyDamageValueViewComponent self,Vector3 startPos,long DamageValue)
        {
            GameObject flyDamageValueGameObject = GameObjectPoolHelper.GetObjectFromPool("flyDamageValue");
            flyDamageValueGameObject.transform.SetParent(GlobalComponent.Instance.Unit);
            self.FlyingDamageSet.Add(flyDamageValueGameObject);
            flyDamageValueGameObject.SetActive(true);
            
            flyDamageValueGameObject.GetComponentInChildren<TextMeshPro>().text = DamageValue <= 0? "Miss" : $"-{DamageValue}";
            flyDamageValueGameObject.transform.position =startPos;
            
            flyDamageValueGameObject.transform.DOMoveZ(startPos.z + 1.5f, 0.8f).onComplete = () =>
            {
                flyDamageValueGameObject.SetActive(false);
                self.FlyingDamageSet.Remove(flyDamageValueGameObject);
                GameObjectPoolHelper.ReturnObjectToPool(flyDamageValueGameObject);
            };
            await ETTask.CompletedTask;
        }
        
    }
}