using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FirstValheimMod
{
    public class AssetLoader
    {
        
        AssetBundle bundle;

        public void InitAssetBundle(string assetBundleName)
        {
            string modFolder = Path.Combine(Paths.PluginPath, "FirstValheimMod");
            string bundlePath = Path.Combine(modFolder, "AssetBundles", assetBundleName);
            bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null)
            {
                MyLogger.Error("Failed to load AssetBundle!");
                return;
            }
        }

        public GameObject LoadAsset(string assetName)
        {
           GameObject Asset = bundle.LoadAsset<GameObject>(assetName);

            if (Asset == null)
            {
                MyLogger.Error("Could not load prefab from AssetBundle!");
            }

            return Asset;
        }
    }
}
