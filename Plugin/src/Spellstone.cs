using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;

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
        return humanoid.GetSpellStone() != null;
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
    public readonly SE_SpellStone statusEffect;
    public readonly string englishName;
    public readonly ConfigEntry<Restriction> restrictConfig;
    
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