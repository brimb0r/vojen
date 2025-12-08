using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using Plugin.Accessors;
using Plugin.Util;
using ItemManagers;

namespace Plugin
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("Azumatt.AzuExtendedPlayerInventory", BepInDependency.DependencyFlags.SoftDependency)]
    public class VojenPlugin : BaseUnityPlugin
    {
        internal const string ModName = "Vojen";
        internal const string ModVersion = "0.0.1";
        internal const string Author = "Creg";
        private const string ModGUID = Author + "." + ModName;
        public const string ConfigFileName = ModGUID + ".cfg";
        public static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource VojenLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);
        public static VojenPlugin Instance = null!;
        public static GameObject root = null!;
        public static readonly Dir SpellStoneDir = new (Paths.ConfigPath, "Vojen");

        public static readonly ConfigSync ConfigSync = new(ModGUID)
        { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public static GameObject _Root = null!;
        public bool m_headless = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;

        public void Awake()
        {
            try
            {
                VojenLogger.LogInfo($"Initializing Vojen Plugin");
                Instance = this;
                root = new GameObject("Vojen.Root");
                DontDestroyOnLoad(root);
                root.SetActive(false);
                
                Configs.Setup();
                Keys.Write();
                StartCoroutine(WaitForObjectDB());
                Assembly assembly = Assembly.GetExecutingAssembly();
                _harmony.PatchAll(assembly);
            }
            catch (Exception ex)
            {
                VojenLogger.LogError($"Error in Awake: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private IEnumerator WaitForObjectDB()
        {
            // Wait until ObjectDB.instance is initialized
            while (ObjectDB.instance == null)
            {
                yield return null; // Wait for the next frame
            }

            // Wait until Player.m_localPlayer is initialized
            while (Player.m_localPlayer == null)
            {
                yield return null; // Wait for the next frame
            }
            
            InitAzuPlugin();
            InitEngine();
        }

        private void InitAzuPlugin()
        {
            VojenLogger.LogInfo("Initializing Azu Plugin");
            if (!AzuExtendedPlayerInventory.API.IsLoaded())
            {
                return;
            }

            // Check if ObjectDB.instance is null
            if (ObjectDB.instance == null)
            {
                VojenLogger.LogInfo("ObjectDB.instance is null. Ensure the ObjectDB is initialized before accessing it.");
                return;
            }
        }

        private void InitEngine()
        {
            VojenLogger.LogInfo("Initializing Vojen Engine");
            Engine.SpellStone.Init();
        }

        private void OnDestroy()
        {
            Config.Save();
        }
    }
}