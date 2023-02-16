using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace ET
{
    public class AssetsBundleHelper
    {
          public static async ETTask<Scene> LoadSceneAsync(string assetName)
        {
            SceneOperationHandle handle = YooAssets.LoadSceneAsync(assetName);
            await handle.Task;
            return handle.SceneObject;
        }
        
        public static async ETTask<Object> LoadOneAssetAsync(string assetName)
        {
            AssetOperationHandle handle = YooAssets.LoadAssetAsync<Object>(assetName);
            await handle.Task;
            return handle.AssetObject;
        }
        
        public static async ETTask<T> LoadOneSubAssetAsync<T>(string location,string assetName) where T:Object
        {
            SubAssetsOperationHandle handle = YooAssets.LoadSubAssetsAsync<T>(location);
            await handle.Task;
            return handle.GetSubAssetObject<T>(assetName);
        }
        
        public static  T LoadOneSubAssetSync<T>(string location,string assetName) where T:Object
        {
            SubAssetsOperationHandle handle = YooAssets.LoadSubAssetsSync<T>(location);
            return handle.GetSubAssetObject<T>(assetName);
        }
        
        public static Object LoadOneAssetSync(string assetName)
        {
            AssetOperationHandle handle = YooAssets.LoadAssetSync<Object>(assetName);
            if (handle.AssetObject != null)
            {
                return handle.AssetObject;
            }
            return null;
        }
        
        public static Dictionary<string, Object> LoadSomeAssetSync(string[] assetNames)
        {
            Dictionary<string, Object> objects = new Dictionary<string, Object>();
            for (int i = 0; i < assetNames.Length; i++)
            {
                AssetOperationHandle handle = YooAssets.LoadAssetSync<Object>(assetNames[i]);
                if (handle.AssetObject != null)
                {
                    objects.Add(handle.AssetObject.name,handle.AssetObject);
                }
               
            }
            return objects;
        }

        public static Dictionary<string, Object> LoadSomeAssetSyncByTag(string tag)
        {
            Dictionary<string, Object> objects = new Dictionary<string, Object>();
            AssetInfo[] assetInfos = YooAssets.GetAssetInfos(tag);
            foreach (AssetInfo assetInfo in assetInfos)
            {
                Object o = LoadOneAssetSync(assetInfo.Address);
                objects.Add(assetInfo.Address,o);
            }
            return objects;
        }

        public static Dictionary<string, byte[]> LoadSomeByteAssetSyncByTag(string tag)
        {
            Dictionary<string, byte[]> objects = new Dictionary<string, byte[]>();
            AssetInfo[] assetInfos = YooAssets.GetAssetInfos(tag);
            foreach (AssetInfo assetInfo in assetInfos)
            {
                byte[] o = ((TextAsset)LoadOneAssetSync(assetInfo.Address)).bytes;
                objects.Add(assetInfo.Address,o);
            }
            return objects;
        }
        
        public static void unloadAsset()
        {
            var package = YooAssets.GetAssetsPackage("DefaultPackage");
            package.UnloadUnusedAssets();
        }
        
    }
}