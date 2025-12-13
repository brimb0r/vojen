using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Patches;

public class Sage
{
    [HarmonyPatch(typeof(AlmanacClasses.Classes.Abilities.Sage.CallOfLightning), "DelayedCast")]
    private static class CallOfLightning_DelayedCast_Patch
    {
        private static void Prefix(float delay, AlmanacClasses.Classes.Talent talent, List<Character> characters)
        {
            var player = Player.m_localPlayer;
            if (player == null) return;

            // Check if the player has the SpellStone equipped
            bool hasSpellStoneEquipped = player.GetInventory()
                .GetEquippedItems()
                .Any(item => item.m_shared.m_name == "SpellStone");

            // Get intelligence value
            float intelligence = AlmanacClasses.API.API.GetCharacteristic("Intelligence");

            // Retrieve the base damage
            var baseDamages = talent.GetDamages(talent.GetLevel());
            VojenPlugin.VojenLogger.LogInfo($"Base Lightning Damage: {baseDamages.m_lightning}");

            // Modify the talent's damage dynamically
            if (hasSpellStoneEquipped)
            {
                baseDamages.m_lightning *= 1.5f; // Example: Increase lightning damage by 50%
                baseDamages.m_lightning += intelligence * 0.2f; // Add 20% of intelligence to lightning damage
            }

            // Log the modified damage
            VojenPlugin.VojenLogger.LogInfo($"Modified Lightning Damage: {baseDamages.m_lightning}");
        }
    }
}