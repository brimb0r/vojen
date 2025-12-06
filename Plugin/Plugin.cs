using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;

namespace Plugin
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class VojenPlugin : BaseUnityPlugin
    {
        internal const string ModName = "Vojen";
        internal const string ModVersion = "0.0.1";
        internal const string Author = "Creg";
        private const string ModGUID = Author + "." + ModName;
        private const string ConfigFileName = ModGUID + ".cfg";

        private static readonly string ConfigFileFullPath =
            Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource VojenLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);

        public static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public static GameObject _Root = null!;
        public bool m_headless = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;

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
        }

        public void Awake()
        {
            try
            {
                InitializePlugin();
                StartCoroutine(WaitForObjectDB());
            }
            catch (Exception ex)
            {
                VojenLogger.LogError($"Error in Awake: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void InitializePlugin()
        {
            VojenLogger.LogInfo($"Initializing Vojen Plugin");
        }
    }
}