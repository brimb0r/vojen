using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static Plugin.VojenPlugin;

namespace Plugin.Accessors;

public class SpriteAccessor
{
    public static readonly Sprite SpellStone = RegisterSprite("spellstone.png", "Icons.core");

    private static Sprite RegisterSprite(string fileName, string folderName = "Icons")
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string path = $"Plugin.{folderName}.{fileName}";

        Debug.Log($"Attempting to register sprite: {path}");

        try
        {
            using var stream = assembly.GetManifestResourceStream(path);
            if (stream == null)
            {
                Debug.LogError($"Resource stream not found for sprite: {path}");
                return null;
            }

            byte[] buffer = new byte[stream.Length];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            Debug.Log($"Read {bytesRead} bytes for sprite: {path}");

            Texture2D texture = new Texture2D(2, 2);
            if (!texture.LoadImage(buffer))
            {
                Debug.LogError($"Failed to load image data for sprite: {path}");
                return null;
            }

            Debug.Log($"Successfully registered sprite: {path}");

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error registering sprite '{path}': {ex.Message}");
            return null;
        }
    }
}