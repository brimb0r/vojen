using BepInEx.Configuration;
using Plugin.Accessors;
using Plugin.Util;
using ItemManagers;

namespace Plugin.Engine;

public class SpellStone
{
    public enum Toggle { On = 1, Off = 0 }
    public static void Init()
    {
        VojenPlugin.VojenLogger.LogInfo("Lets add the spell stone.");

        if (ObjectDB.instance == null) return;
        if (!AzuExtendedPlayerInventory.API.IsLoaded()) return ;
        var spellStonePrefab = ObjectDB.instance.GetItemPrefab("Hammer");
        var instantiatedPrefab = ObjectDB.Instantiate(spellStonePrefab);
        
        Item SpellStone = new Item(instantiatedPrefab);
        SpellStone.Name.English("SpellStone");
        SpellStone.Description.English("Magicstone that can store and cast spells.");
        SpellStone.RequiredItems.Add("Stone", 5);
        SpellStone.RequiredItems.Add("DeerHide", 2);
        SpellStone.RequiredItems.Add("Resin", 2);
        SpellStone.RequiredItems.Add("Wood", 2);
        SpellStone.RequiredUpgradeItems.Add("LeatherScraps", 10, 2);
        SpellStone.RequiredUpgradeItems.Add("DeerHide", 5, 2);
        SpellStone.RequiredUpgradeItems.Add("Resin", 8, 2);
        SpellStone.RequiredUpgradeItems.Add("BoneFragments", 10, 3);
        SpellStone.RequiredUpgradeItems.Add("DeerHide", 5, 3);
        SpellStone.RequiredUpgradeItems.Add("Copper", 5, 3);
        SpellStone.RequiredUpgradeItems.Add("BjornHide", 10, 4);
        SpellStone.RequiredUpgradeItems.Add("DeerHide", 5, 4);
        SpellStone.RequiredUpgradeItems.Add("Bronze", 5, 4);
        SpellStone.Crafting.Add(CraftingTable.Workbench, 1);
        var spellStoneSetup = new SpellStoneSetup(SpellStone, 5, 2);
        spellStoneSetup.statusEffect.m_baseCarryWeight = 5f;
        ConfigEntry<Toggle> addSpellStoneConfig = Configs.config("2 - Extended Player Inventory", "Add SpellStoneSlot", Toggle.On, "If on, will add SpellStone to EPI");
        string spellStoneLabel = Keys.SpellStone;
        ObjectDB.instance.m_items.Add(instantiatedPrefab);
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
}