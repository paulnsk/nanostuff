using TextCleaner.BLL.Models;

namespace TextCleaner.BLL.Interfaces;

public interface IFileProcessingService
{
    event EventHandler<IEnumerable<TextCleanerJob>>? QueueChanged;
    event EventHandler<ProcessingProgressEventArgs>? Progress;
    void Start();
    void Enqueue(TextCleanerJob job);
    Task ProcessQueueAsync(CancellationToken token);
    void ProcessFile(TextCleanerJob job, CancellationToken token);
    void Stop();
}