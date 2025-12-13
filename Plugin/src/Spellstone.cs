using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Managers;
using UnityEngine;
using Plugin.Accessors;

namespace Plugin;


public static class SpellStoneExtensions
{
    public static bool IsSpellStone(this ItemDrop.ItemData item, out SpellStoneSetup setup) => SpellStoneSetup.spellstone.TryGetValue(item.m_shared.m_name, out setup);

    public static SpellStone? GetEquippedSpellStone(this Humanoid humanoid)
    {
        foreach (ItemDrop.ItemData? item in humanoid.GetInventory().GetAllItems())
        {
            if (item is SpellStone spellstone)
            {
                return spellstone;
            }
        }
        return null;
    }

    public static SpellStone? GetSpellStone(this Humanoid humanoid)
    {
        foreach (ItemDrop.ItemData? item in humanoid.GetInventory().GetAllItems())
        {
            if (item is SpellStone spellstone) return spellstone;
        }
        return null;
    }

    public static bool HasSpellStone(this Humanoid humanoid)
    {
        return true;
    }
}

public class SpellStoneSetup
{
    [Flags]
    public enum Restriction
    {
        None = 0, 
        NoMaterials = 1, 
        NoConsumables = 2, 
        NoTrophies = 4, 
    }
    
    public static readonly Dictionary<string, SpellStoneSetup> spellstone = new();
    public readonly Dictionary<int, Size> sizes = new();
    public readonly SE_SpellStone statusEffect;
    public readonly string englishName;
    public readonly ConfigEntry<Restriction> restrictConfig;

    public SpellStoneSetup(Jotunn.Entities.CustomItem item, int width, int height)
    {
        VojenPlugin.VojenLogger.LogInfo("SpellStone Setup complete.");
    }

    public void AddSizePerQuality(int width, int height, int quality)
    {
        sizes[quality] = new Size(width, height);
    }

    public void SetupConfigs()
    {
        foreach (KeyValuePair<int, Size> size in sizes)
        {
            ConfigEntry<string> config = Configs.config(englishName, $"Inventory Size Qlty.{size.Key}",
                string.Join("x", size.Value.width, size.Value.height), new ConfigDescription(
                    $"Setup inventory size for quality {size.Key}, width x height", null, new Configs.ConfigurationManagerAttributes()
                    {
                        CustomDrawer = Size.Draw
                    }));
            config.SettingChanged += (_, _) => OnConfigChange();
            OnConfigChange();
            
            void OnConfigChange()
            {
                string[] values = config.Value.Split('x');
                if (values.Length != 2) return;
                if (!int.TryParse(values[0], out int width) || !int.TryParse(values[1], out int height)) return;
                sizes[size.Key].width = width;
                sizes[size.Key].height = height;
            }
        }
    }

    public class Size
    {
        public int width;
        public int height;

        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public static void Draw(ConfigEntryBase cfg)
        {
            bool locked = cfg.Description.Tags
                .Select(a =>
                    a.GetType().Name == "ConfigurationManagerAttributes"
                        ? (bool?)a.GetType().GetField("ReadOnly")?.GetValue(a)
                        : null).FirstOrDefault(v => v != null) ?? false;
            
            var values = ((string)cfg.BoxedValue).Split('x');
            if (values.Length != 2) return;
            
            string widthCfg = values[0].Trim();
            string heightCfg = values[1].Trim();
            
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            string width = GUILayout.TextField(widthCfg, new GUIStyle(GUI.skin.textField));
            string height = GUILayout.TextField(heightCfg, new GUIStyle(GUI.skin.textField));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            if (!locked && (width != widthCfg || height != heightCfg))
            {
                cfg.BoxedValue = string.Join("x", width, height);
            }
        }
    }
    
    [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.Awake))]
    private static class ItemDrop_Awake_Patch 
    {
        [UsedImplicitly]
        private static void Prefix(ItemDrop __instance)
        {
            if (!__instance.m_itemData.IsSpellStone(out SpellStoneSetup setup)) return;
        }
    }
}

public class SpellStone : ItemDrop.ItemData
{
    private readonly float baseWeight;
    
    public SpellStone(ItemDrop.ItemData item)
    {
        m_shared = item.m_shared;
        m_customData = item.m_customData;
        baseWeight = m_shared.m_weight;
    }
}

public class SE_SpellStone : SE_Stats
{
    public float m_baseCarryWeight;
}