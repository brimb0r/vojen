using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using Jotunn;
using System.Threading.Tasks;
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

                VojenLogger.LogInfo("Lets Add the SpellStone.");
                var spellStonePrefab = ObjectDB.instance.GetItemPrefab("Torch");
                var instantiatedPrefab = Instantiate(spellStonePrefab);
                instantiatedPrefab.name = "SpellStone";
                var itemDrop = instantiatedPrefab.GetComponent<ItemDrop>();
                itemDrop.m_itemData.m_shared.m_name = "SpellStone";
                itemDrop.m_itemData.m_shared.m_description = "A magical stone imbued with arcane power.";
                itemDrop.m_itemData.m_shared.m_maxStackSize = 1;
                itemDrop.m_itemData.m_shared.m_weight = 1f;
                itemDrop.m_itemData.m_shared.m_value = 100;
                itemDrop.m_itemData.m_shared.m_icons = new[] { SpriteAccessor.SpellStone };
                itemDrop.m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Trinket;
                itemDrop.m_itemData.m_shared.m_damages = new HitData.DamageTypes();
                itemDrop.m_itemData.m_shared.m_damagesPerLevel = new HitData.DamageTypes();
               
                ObjectDB.instance.m_items.Add(instantiatedPrefab);
                VojenLogger.LogInfo($"Added prefab to ObjectDB: {instantiatedPrefab.name}");
                VojenLogger.LogInfo($"SpellStone prefab exists: {ObjectDB.instance.GetItemPrefab("SpellStone") != null}");
                VojenLogger.LogInfo("Lets Configure the SpellStone.");
                var recipe = ScriptableObject.CreateInstance<Recipe>();
                recipe.name = "Recipe_SpellStone";
                recipe.m_item = instantiatedPrefab.GetComponent<ItemDrop>(); 
                recipe.m_amount = 1; // Amount of the item crafted per recipe
                
                recipe.m_resources = new[]
                {
                    new Piece.Requirement
                    {
                        m_resItem = ObjectDB.instance.GetItemPrefab("Wood")?.GetComponent<ItemDrop>(), // Example: requires Wood
                        m_amount = 5 
                    },
                    new Piece.Requirement
                    {
                        m_resItem = ObjectDB.instance.GetItemPrefab("Stone")?.GetComponent<ItemDrop>(), // Example: requires Stone
                        m_amount = 2 
                    }
                };
                
                ObjectDB.instance.m_recipes.Add(recipe);
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
        
        public void SetupEPI()
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
                    AzuExtendedPlayerInventory.API.AddSlot(spellStoneLabel, player => player.GetEquippedSpellStone(),
                        item =>
                        {
                            if (item is not Plugin.SpellStone) return false;
                            return !(Player.m_localPlayer?.HasSpellStone() ?? false);
                        });
                }
            }
           
        }
        private async Task WaitForObjectDB()
        {
            while (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0)
            {
                await Task.Yield();
            }

            while (Player.m_localPlayer == null)
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