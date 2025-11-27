using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;

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
    }
}