using System.Reflection;
using System.Text.Json;
using TextCleaner.BLL.Interfaces;
using TextCleaner.BLL.Models;

namespace TextCleaner.BLL.Services;

public class PersistentQueueStorageService : IPersistentQueueStorageService
{
    private readonly string _storageFilePath;

    public PersistentQueueStorageService()
    {
        var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var dataDirectory = Path.Combine(exePath!, "data");
        Directory.CreateDirectory(dataDirectory);
        _storageFilePath = Path.Combine(dataDirectory, "queue.json");
    }

    
    public IEnumerable<TextCleanerJob> Load()
    {
        if (!File.Exists(_storageFilePath))
        {
            return [];
        }

        var json = File.ReadAllText(_storageFilePath);
        return JsonSerializer.Deserialize<List<TextCleanerJob>>(json) ?? [];
    }
    
    public void Save(IEnumerable<TextCleanerJob?> queueData)
    {
        File.WriteAllText(_storageFilePath, JsonSerializer.Serialize(queueData));
    }
}