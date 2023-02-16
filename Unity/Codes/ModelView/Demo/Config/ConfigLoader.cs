using YooAsset;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
       public class ConfigLoader: IConfigLoader
    {
        public Dictionary<string, byte[]> GetAllConfigBytes()
        {
            return AssetsBundleHelper.LoadSomeByteAssetSyncByTag("config");
        }

        public byte[] GetOneConfigBytes(string configName)
        {
            return  ((TextAsset)AssetsBundleHelper.LoadOneAssetSync("configName")).bytes;
        }
    }
}