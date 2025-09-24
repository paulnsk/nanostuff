using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using TextCleaner.BLL.Interfaces;
using TextCleaner.BLL.Models;
using TextCleaner.BLL.Utilities;

namespace TextCleaner.BLL.Services;

public class FileProcessingService
    (
        IPersistentQueueStorageService storageService, 
        ILogger<FileProcessingService> logger,
        TextCleaner textCleaner
        ) : IFileProcessingService
{
    private ConcurrentQueue<TextCleanerJob> _queue = [];
    private readonly CancellationTokenSource _cts = new();
    private Task? _processingTask;
    
    
    // Здесь бы лучше использовать какой-нибудь pub-sub, например от MediatR, но это за рамками тестового задания. Ивенты сойдут.
    public event EventHandler<IEnumerable<TextCleanerJob>>? QueueChanged;
    public event EventHandler<ProcessingProgressEventArgs>? Progress;

    public void Start()
    {
        var items = storageService.Load();
        _queue = new ConcurrentQueue<TextCleanerJob>(items);
        _processingTask = Task.Run(() => ProcessQueueAsync(_cts.Token));
        QueueChanged?.Invoke(this, _queue.ToArray());
    }
    
    public void Enqueue(TextCleanerJob job)
    {
        _queue.Enqueue(job);
        storageService.Save(_queue.ToArray());
        QueueChanged?.Invoke(this, _queue.ToArray());
    }

    public async Task ProcessQueueAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (_queue.TryDequeue(out var job))
                {
                    ProcessFile(job, token);
                    QueueChanged?.Invoke(this, _queue.ToArray());
                    storageService.Save(_queue.ToArray());
                }
                else
                {
                    await Task.Delay(100, token);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred in the processing queue.");
            }
        }
    }

    public void ProcessFile(TextCleanerJob job, CancellationToken token)
    {
        logger.LogInformation("Starting processing job for source: {SourceFile}", job.SourceFilePath);
        token.ThrowIfCancellationRequested();

        try
        {
            using var sourceStream = new FileStream(job.SourceFilePath, FileMode.Open, FileAccess.Read);
            Utils.EnsureDir(job.TargetFilePath);
            using var targetStream = new FileStream(job.TargetFilePath, FileMode.Create, FileAccess.Write);

            var reader = new BlockStreamReader(sourceStream);
            var writer = new BlockStreamWriter(targetStream);

            var totalBytes = sourceStream.Length;
            long processedBytes = 0;
            var fileName = Path.GetFileName(job.SourceFilePath);
            
            Progress?.Invoke(this, new ProcessingProgressEventArgs(fileName, totalBytes, 0));

            foreach (var block in reader.ReadBlocks(token))
            {
                var cleanedBlock = textCleaner.CleanText(block, job.MinWordLength, job.ItemsToRemove);
                writer.WriteBlock(cleanedBlock);
                
                processedBytes += Encoding.UTF8.GetByteCount(block);
                Progress?.Invoke(this, new ProcessingProgressEventArgs(fileName, totalBytes, processedBytes));
            }
            
            Progress?.Invoke(this, new ProcessingProgressEventArgs(fileName, totalBytes, totalBytes));
            logger.LogInformation("Successfully finished processing job for source: {SourceFile}", job.SourceFilePath);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Processing was canceled for source: {SourceFile}. Target file may be incomplete.", job.SourceFilePath);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to process file: {SourceFile}, {ex}", job.SourceFilePath, ex.Message);
        }
    }

    public void Stop()
    {
        _cts.Cancel();
        _processingTask?.Wait();
    }
}