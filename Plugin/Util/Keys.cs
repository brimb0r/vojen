using System.Collections.Generic;
using System.Linq;


namespace Plugin.Util;

public static class Keys
{    private static readonly Dictionary<string, string> keys = new();
    
    public static void Write()
    {
        List<string> lines = new();
        foreach (KeyValuePair<string, string> kvp in keys.OrderBy(x => x.Key))
        {
            lines.Add($"{kvp.Key}: \"{kvp.Value}\"");
        }
        VojenPlugin.SpellStoneDir.WriteAllLines($"{VojenPlugin.ModName}.English.yml", lines);
    }

    public class Key
    {
        public readonly string key;

        public Key(string key, string english)
        {
            this.key = key;
            keys[key.Replace("$", string.Empty)] = english;
        }
    }
    
    public static readonly string SpellStone = new Key("SpellStone", "SpellStone").key;
    
}