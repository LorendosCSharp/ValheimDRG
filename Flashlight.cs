using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace FirstValheimMod
{
    public class Flashlight : MonoBehaviour
    {
        AssetLoader loader;
        GameObject FlareAsset = null;

        private List<PooledFlare> pool = new List<PooledFlare>();
        private int poolSize = 20;
        private int currentIndex = 0; // Tracks which flare to use next

        public void Awake()
        {
            loader = new AssetLoader();
            loader.InitAssetBundle("valheimdrg");
            FlareAsset = loader.LoadAsset("Flare");

        }


        public void SpawnFlare(Vector3 spawnPosition, Vector3 direction, float force = 15f)
        {
            if (pool.Count == 0)
            {
                MyLogger.Error("Pool is null!");
                return;
            }
            MyLogger.Error(pool.Count.ToString());
            // Get the next flare in the pool
            PooledFlare flare = pool[currentIndex];

            if (flare.GO == null)
            {
                MyLogger.Error("Pooled flare or its GameObject is null!");
                return;
            }

            // Deactivate it first if it was still active
            if (flare.GO.activeSelf)
            {
                flare.GO.SetActive(false);
                if (flare.Rigidbody != null)
                {
                    flare.Rigidbody.velocity = Vector3.zero;
                    flare.Rigidbody.angularVelocity = Vector3.zero;
                }
            }

            // Position, rotate, activate
            flare.GO.transform.position = spawnPosition;
            flare.GO.transform.rotation = Quaternion.LookRotation(direction.normalized);
            flare.GO.SetActive(true);

            if (flare.Rigidbody != null)
                flare.Rigidbody.AddForce(direction.normalized * force, ForceMode.Impulse);

            // Move index to next flare for next spawn
            currentIndex = (currentIndex + 1) % pool.Count;
        }

        #region Pool
        public void InstantiateFlares(float flareStenght, string flareColor)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject flareGO = Instantiate(FlareAsset, transform);

                if (flareGO == null)
                {
                    MyLogger.Error("Failed to instantiate flare GameObject!");
                    continue;
                }

                /*
                 Creating Pooled flare for easier storage of the flares childs (Light for now)
                Initializing the flares 
                 */

                PooledFlare pf = new PooledFlare(flareGO);

                if (pf.GO == null)
                {
                    MyLogger.Warn("Instantiated Flare does not have a GmaObject");
                }

                if (!ColorUtility.TryParseHtmlString(flareColor, out Color c))
                {
                    MyLogger.Warn("Invalid flare color: " + flareColor);
                    return;
                }

                Renderer renderer = pf.GO.GetComponent<Renderer>();
                if (renderer != null && renderer.materials.Length > 1)
                {
                    Material[] materials = renderer.materials;
                    materials[1].color = c;
                    renderer.materials = materials;
                }

                if (pf.Light != null)
                {
                    pf.Light.intensity = flareStenght;
                    pf.Light.color = c;
                }

                MyLogger.Warn("Added a polledFlare to pool");
                pool.Add(pf);
            }
        }
        #endregion
    }

    public class PooledFlare
    {
        public GameObject GO;
        public Rigidbody Rigidbody;
        public Light Light;

        public PooledFlare(GameObject go)
        {
            GO = go;
            Rigidbody = go.GetComponent<Rigidbody>();
            Light = go.GetComponentInChildren<Light>();
            GO.SetActive(false);
        }
    }
}
