using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using JetBrains.Annotations;

namespace Plugin.Accessors;

public class Configs
{
    public enum Toggle { On = 1, Off = 0 }
    
    public static void Setup()
    {
        
        SetupWatcher();
    }
    
    private static void SetupWatcher()
    {
        FileSystemWatcher watcher = new(Paths.ConfigPath, VojenPlugin.ConfigFileName);
        watcher.Changed += ReadConfigValues;
        watcher.Created += ReadConfigValues;
        watcher.Renamed += ReadConfigValues;
        watcher.IncludeSubdirectories = true;
        watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        watcher.EnableRaisingEvents = true;
    }
    
    private static void ReadConfigValues(object sender, FileSystemEventArgs e)
    {
        if (!File.Exists(VojenPlugin.ConfigFileFullPath)) return;
        try
        {
            VojenPlugin.VojenLogger.LogDebug("ReadConfigValues called");
            VojenPlugin.Instance.Config.Reload();
        }
        catch
        {
            VojenPlugin.VojenLogger.LogError($"There was an issue loading your {VojenPlugin.ConfigFileName}");
            VojenPlugin.VojenLogger.LogError("Please check your config entries for spelling and format!");
        }
    }
    
    public static ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
        bool synchronizedSetting = true)
    {
        ConfigDescription extendedDescription =
            new(
                description.Description +
                (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                description.AcceptableValues, description.Tags);
        ConfigEntry<T> configEntry = VojenPlugin.Instance.Config.Bind(group, name, value, extendedDescription);
        

        return configEntry;
    }
    
    public static ConfigEntry<T> config<T>(string group, string name, T value, string description,
        bool synchronizedSetting = true)
    {
        return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
    }
    
    public class ConfigurationManagerAttributes
    {
        [UsedImplicitly] public int? Order = null!;
        [UsedImplicitly] public bool? Browsable = null!;
        [UsedImplicitly] public string? Category = null!;
        [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer = null!;
    }
    
}