using System;
using System.Collections.Generic;
using System.IO;


namespace Plugin.Util;

public class Dir 
{
    public readonly string Path;
    public Dir(string dir, string name)
    {
        Path = System.IO.Path.Combine(dir, name);
        EnsureDirectoryExists();
    }

    private void EnsureDirectoryExists()
    {
        if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
    }

    public void WriteAllLines(string fileName, List<string> lines)
    {
        var fullPath = System.IO.Path.Combine(Path, fileName);
        ExecuteWithRetry(() => File.WriteAllLines(fullPath, lines));   
    }

    private void ExecuteWithRetry(Action operation)
    {
        try
        {
            operation();
        }
        catch (DirectoryNotFoundException)
        {
            EnsureDirectoryExists();
            operation();
        }
    }
}