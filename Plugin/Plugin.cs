using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Linq;
using Jotunn;
using System.Threading.Tasks;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Plugin.Accessors;
using Plugin.Util;

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

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public static GameObject _Root = null!;
        public bool m_headless = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;

        public async void Awake()
        {
            try
            {
                VojenLogger.LogInfo($"Initializing Vojen Plugin");
                Instance = this;
                root = new GameObject("Vojen.Root");
                DontDestroyOnLoad(root);
                root.SetActive(false);
                await WaitForObjectDB();
                AddConsoleCommands();

                VojenLogger.LogInfo("Lets Add the SpellStone.");
                AddSpellStone();
                AddRecipes();
                VojenLogger.LogInfo("Added the SpellStone.");
                Configs.Setup();
                Keys.Write();
                SetupEPI();
                Assembly assembly = Assembly.GetExecutingAssembly();
                _harmony.PatchAll(assembly);
            }
            catch (Exception ex)
            {
                VojenLogger.LogError($"Error in Awake: {ex.Message}\n{ex.StackTrace}");
            }
        } 
        
        private void AddConsoleCommands()
        {
            Jotunn.Managers.CommandManager.Instance.AddConsoleCommand(new ConsoleCommands.PrintItemsCommand());
            Jotunn.Managers.CommandManager.Instance.AddConsoleCommand(new ConsoleCommands.TpCommand());
            Jotunn.Managers.CommandManager.Instance.AddConsoleCommand(new ConsoleCommands.ListPlayersCommand());
            Jotunn.Managers.CommandManager.Instance.AddConsoleCommand(new ConsoleCommands.BetterSpawnCommand());
        }
        
        private void AddRecipes()
        {
        }

        // Custom status effect
        private CustomStatusEffect SpellStoneEffect;
        private ButtonConfig SpellStoneSpecialButton;
        private SpellStone SpellStone;
        
        private void AddSpellStone()
        {
            try
            {
                // Check if the base prefab exists
                GameObject basePrefab = ObjectDB.instance.GetItemPrefab("Torch");
                if (basePrefab == null)
                {
                    VojenLogger.LogError("Base prefab 'Torch' not found in ObjectDB.");
                    return;
                }

                // Check if the sprite is loaded
                if (SpriteAccessor.SpellStone == null)
                {
                    VojenLogger.LogError("SpriteAccessor.SpellStone is null. Ensure the sprite is loaded correctly.");
                    return;
                }


                // Create and add a custom item based on Torch
                ItemConfig spellStone = new ItemConfig
                {
                    Name = "SpellStone",
                    Description = "Use This to Cast Spells",
                    Icon = SpriteAccessor.SpellStone,
                    CraftingStation = "piece_workbench",
                    MinStationLevel = 1
                };
                spellStone.AddRequirement(new RequirementConfig("Stone", 1));

                CustomItem customItem = new CustomItem("SpellStone", "Flint", spellStone);
                ItemManager.Instance.AddItem(customItem);

                // Add custom status effect
                customItem.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = SpellStoneEffect?.StatusEffect;
                new SpellStone(customItem.ItemDrop.m_itemData);
                new SpellStoneSetup(customItem,5, 0);

                // Unsubscribe from the event to avoid duplicate calls
                PrefabManager.OnVanillaPrefabsAvailable -= AddSpellStone;

                VojenLogger.LogInfo("SpellStone added successfully.");
            }
            catch (Exception ex)
            {
                VojenLogger.LogError($"Error in AddSpellStone: {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        private void SetupEPI()
        {
            VojenLogger.LogInfo("Initializing EPI");
            if (!AzuExtendedPlayerInventory.API.IsLoaded()) return;
            ConfigEntry<Toggle> addSpellStoneConfig = Configs.config("2 - Extended Player Inventory", "Add SpellStoneSlot", Toggle.On, "If on, will add SpellStone to EPI");
            string spellStoneLabel = Keys.SpellStone;
            OnSpellStoneConfigChange();
            void OnSpellStoneConfigChange()
            {
                AzuExtendedPlayerInventory.API.RemoveSlot(spellStoneLabel);
                if (addSpellStoneConfig.Value is Toggle.On)
                {
                    AzuExtendedPlayerInventory.API.AddSlot(
                        spellStoneLabel,
                        player => player.GetInventory().GetAllItems().FirstOrDefault(i => i != null && i.m_dropPrefab
                            && i.m_shared.m_name == "SpellStone"),
                        item => item?.m_shared?.m_name == "SpellStone" && !(Player.m_localPlayer?.HasSpellStone() ?? false)
                    );
                }
            }
           
        }
        
        
        private async Task WaitForObjectDB()
        {
            while (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0)
            {
                await Task.Yield();
            }

            VojenLogger.LogInfo("ObjectDB.instance is initialized.");
        }

        private void OnDestroy()
        {
            Config.Save();
        }
    }
}