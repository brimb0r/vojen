using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using UnityEngine;
using System;

namespace vojen.mods
{
    [BepInPlugin("vojen.swimmingRegen", "Swimming Stamina Regen", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class SwimmingStaminaRegen : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("vojen.swimmingRegen");
        public static ConfigEntry<float> TreadingStaminaRegen;

        void Awake()
        {
            TreadingStaminaRegen = Config.Bind("General", "TreadingStaminaRegen", 1f, "Treading Stamina Regen");

            harmony.PatchAll();
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
        
        [HarmonyPatch(typeof(Player), "UpdateStats", typeof(float))]
        static class Player_UpdateStats_Patch
        {
            static void Postfix(float dt, Character __instance, ref float ___m_stamina)
            {
                if (__instance.IsSwimming() && !__instance.IsOnGround())
                {
                    ___m_stamina = Mathf.Min(__instance.GetMaxStamina(), ___m_stamina + TreadingStaminaRegen.Value * dt);
                }
            }
        }
    }
}