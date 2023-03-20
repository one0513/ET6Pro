using UnityEngine;
using DG.Tweening;

namespace ET
{
    public class EffectComponentAwakeSystem : AwakeSystem<EffectComponent>
    {
        public override void Awake(EffectComponent self)
        {
            self.Awake().Coroutine();
        }
    }
    
    
    public class EffectComponentDestroySystem : DestroySystem<EffectComponent>
    {
        public override void Destroy(EffectComponent self)
        {

        }
    }
    
    
    public static class EffectComponentSystem
    {
        
        public static async ETTask Awake(this EffectComponent self)
        {
            GameObject thunderAtk =  AssetsBundleHelper.LoadOneAssetSync("thunderAtk") as GameObject;
            
            await GameObjectPoolHelper.InitPoolFormGamObjectAsync(thunderAtk ,3);
            
            GameObject thunderAtkHit =  AssetsBundleHelper.LoadOneAssetSync("thunderAtkHit") as GameObject;
            
            await GameObjectPoolHelper.InitPoolFormGamObjectAsync(thunderAtkHit ,3);
        }
        
        public static async ETTask ShowEffect(this EffectComponent self,Vector3 fromUPos,Vector3 toUPos)
        {
            
            GameObject atk = GameObjectPoolHelper.GetObjectFromPool("thunderAtk");
            atk.transform.position = fromUPos;
            atk.transform.SetParent(GlobalComponent.Instance.Unit);
            atk.SetActive(true);
            
            
            GameObject hit = GameObjectPoolHelper.GetObjectFromPool("thunderAtkHit");
            hit.transform.SetParent(GlobalComponent.Instance.Unit);
            hit.SetActive(false);
            

            atk.transform.position =fromUPos;
            
            atk.transform.DOMove(new Vector3(toUPos.x,2,toUPos.z), 1f).onComplete = () =>
            {
                atk.SetActive(false);
                GameObjectPoolHelper.ReturnObjectToPool(atk);
                
            };
            await TimerComponent.Instance.WaitAsync(1000);
            hit.transform.position = toUPos;
            hit.SetActive(true);
            await TimerComponent.Instance.WaitAsync(1000);
            hit.SetActive(false);
            GameObjectPoolHelper.ReturnObjectToPool(hit);
        }

        
    }
}