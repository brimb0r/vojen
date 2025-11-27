# Help for Valheim Mod Project

- `dnSpy` - A .NET assembly editor and debugger, useful for inspecting and modifying .NET assemblies. 
- `BepInEx` - A plugin framework for Unity games, allowing for easy modding and plugin management.
- `Harmony` - A library for patching, replacing, and decorating .NET methods at runtime, commonly used in modding.
- `Rider` or `Visual Studio` - Integrated Development Environments (IDEs) for writing and managing C# code.
- `Valheim` - The game for which this mod is being developed.


# Required DLLs

To build and run this project, ensure the following DLLs are included and referenced:

## From BepInEx and Harmony
- `0Harmony20.dll`
- `BepInEx.dll`

## From Valheim Game Files
- `assembly_googleanalytics.dll`
- `assembly_guiutils.dll`
- `assembly_steamworks.dll`
- `assembly_utils.dll`
- `assembly_valheim.dll`
- `Microsoft.CSharp.dll`
- `UnityEngine.dll`
- `UnityEngine.AnimationModule.dll`
- `UnityEngine.AssetBundleModule.dll`
- `UnityEngine.AudioModule.dll`
- `UnityEngine.CoreModule.dll`
- `UnityEngine.IMGUIModule.dll`
- `UnityEngine.InputLegacyModule.dll`
- `UnityEngine.InputModule.dll`
- `UnityEngine.JSONSerializeModule.dll`
- `UnityEngine.PhysicsModule.dll`
- `UnityEngine.ScreenCaptureModule.dll`
- `UnityEngine.TextRenderingModule.dll`
- `UnityEngine.UI.dll`
- `UnityEngine.UIModule.dll`
- `UnityEngine.UnityWebRequestAudioModule.dll`
- `UnityEngine.UnityWebRequestModule.dll`
- `UnityEngine.UnityWebRequestWWWModule.dll`

Ensure these DLLs are placed in the appropriate paths as referenced in the project file.

# DevMachine Setup
1. Install `Rider` or `Visual Studio` on your development machine.
2. Download and install `BepInEx` for Valheim.

# Building the Project
1. Clone the repository to your local machine.
2. Open the project in your preferred IDE (Rider or Visual Studio).
3. Ensure all required DLLs are referenced in the project.
4. Build the project to generate the mod DLL.
5. Place the generated DLL in the appropriate BepInEx plugins folder for Valheim.
6. Launch Valheim to test the mod.
7. Debug and iterate as necessary.






