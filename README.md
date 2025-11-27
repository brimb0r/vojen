# Help for Valheim Mod Project

- `dnSpy` - A .NET assembly editor and debugger, useful for inspecting and modifying .NET assemblies. 
- `BepInEx` - A plugin framework for Unity games, allowing for easy modding and plugin management.
- `Harmony` - A library for patching, replacing, and decorating .NET methods at runtime, commonly used in modding.
- `Rider` or `Visual Studio` - Integrated Development Environments (IDEs) for writing and managing C# code.
- `Valheim` - The game for which this mod is being developed.


# Required DLLs

## TODO : Update this list with exact versions and sources of the DLLs, and any additional dependencies.

To build and run this project, ensure the following DLLs are included and reference:

- `0Harmony.dll`
- `BepInEx.dll`
- `valheim_Data\Managed\assembly_valheim.dll`
- `UnityEngine.dll`
- `UnityEngine.CoreModule.dll`


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


