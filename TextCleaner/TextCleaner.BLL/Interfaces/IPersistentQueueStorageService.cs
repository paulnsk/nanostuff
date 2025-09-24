using TextCleaner.BLL.Models;

namespace TextCleaner.BLL.Interfaces;

public interface IPersistentQueueStorageService
{
    IEnumerable<TextCleanerJob> Load();
    void Save(IEnumerable<TextCleanerJob?> queueData);
}