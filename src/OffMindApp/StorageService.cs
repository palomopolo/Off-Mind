using System;
using System.IO;
using System.Text.Json;

namespace OffMindApp;

public sealed class StorageService
{
    private readonly string _libraryPath;

    public StorageService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var folder = Path.Combine(appData, "OffMind");
        Directory.CreateDirectory(folder);
        _libraryPath = Path.Combine(folder, "library.json");
    }

    public Library LoadLibrary()
    {
        if (!File.Exists(_libraryPath))
        {
            return new Library();
        }

        var json = File.ReadAllText(_libraryPath);
        var library = JsonSerializer.Deserialize<Library>(json, JsonOptions());
        return library ?? new Library();
    }

    public void SaveLibrary(Library library)
    {
        var json = JsonSerializer.Serialize(library, JsonOptions());
        File.WriteAllText(_libraryPath, json);
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }
}
