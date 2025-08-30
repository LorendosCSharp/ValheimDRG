using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstValheimMod
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        const string pluginGUID = "com.lorendos";
        const string pluginName = "DRGFlashlight";
        const string pluginVersion = "0.0.1";

        public static ConfigEntry<KeyboardShortcut> FlareLightTrow;
        public static ConfigEntry<string> FlareLightColor;
        public static ConfigEntry<float> FlareLightStrenght;
        public static ConfigEntry<float> FlareLightRange;

        private readonly Harmony HarmonyInstance = new Harmony(pluginGUID);

        GameObject flashlightGO;
        private Flashlight flashlight;
        



        public void Awake()
        {
            InitConfigs();

            MyLogger.Init(pluginName);
            MyLogger.Info("DRGFlashlight plugin loaded");

            // Subscribe to scene load
            SceneManager.sceneLoaded += OnSceneLoaded;

            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Only initialize when we’re in the main game scene
            if (scene.name == "main") // <- world scene in Valheim
            {
                MyLogger.Warn("World scene loaded, creating FlashlightHandler...");

                flashlightGO = new GameObject("Flashlight Handler");
                SceneManager.MoveGameObjectToScene(flashlightGO, scene);
                flashlight = flashlightGO.AddComponent<Flashlight>();

                flashlight.InstantiateFlares(FlareLightStrenght.Value, FlareLightColor.Value,FlareLightRange.Value);
            }
        }

        public void Update()
        {
            if (Player.m_localPlayer == null || flashlight == null) return;

            if (UnityInput.Current.GetKeyDown(FlareLightTrow.Value.MainKey))
            {
                MyLogger.Info("Flashlight was thrown");
                flashlight.SpawnFlare(Player.m_localPlayer.transform.position + 2 * Vector3.up,GameCamera.instance.transform.forward);
            }
        }


        private void InitConfigs()
        {
            FlareLightTrow = Config.Bind("Flare", "TrowLight", KeyboardShortcut.Deserialize("F"), "Hotkey to throw flashlight");
            FlareLightColor = Config.Bind("Flare", "FlareColor", "#FFAA00", "Color of the light");
            FlareLightStrenght = Config.Bind("Flare","FlareLightStrenght",1.5f,"Strenght of the flare color");
            FlareLightRange = Config.Bind("Flare", "FlareLightRange", 10f,"Range of the flare");

        }

    }
}
