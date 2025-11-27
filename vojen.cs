using BepInEx;

namespace vojen
{
    [BepInPlugin("vojen.main", "Main Mod Loader", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class VojenMain : BaseUnityPlugin
    {
        void Awake()
        {
            // Initialize other mods or perform setup here
            Logger.LogInfo("Vojen mod loader initialized.");
        }
    }
}