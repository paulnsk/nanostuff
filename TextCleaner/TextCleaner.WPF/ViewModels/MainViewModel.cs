using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using TextCleaner.BLL.Models;
using TextCleaner.BLL.Utilities;
using TextCleaner.WPF.Models;
using TextCleaner.BLL.Interfaces;
using TextCleaner.WPF.Interfaces.Logging;

namespace TextCleaner.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IFileProcessingService _fileProcessingService;
    private readonly IUiLogRelayService _logRelayService;
    private readonly TextCleanerConfig _config;

    [ObservableProperty]
    private ObservableCollection<string> _queuedFiles = [];

    [ObservableProperty]
    private ObservableCollection<string> _logMessages = [];

    [ObservableProperty]
    private string _currentProgress = "Готов";

    public MainViewModel(
        IFileProcessingService fileProcessingService,
        IUiLogRelayService logRelayService,
        IOptions<TextCleanerConfig> config)
    {
        _fileProcessingService = fileProcessingService;
        _logRelayService = logRelayService;
        _config = config.Value;
        
        _fileProcessingService.QueueChanged += _fileProcessingService_QueueChanged;
        _fileProcessingService.Progress += _fileProcessingService_Progress;
        _logRelayService.LogReceived += _logRelayService_LogReceived;
    }

    private void _logRelayService_LogReceived(string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            LogMessages.Add(message); 
            if (LogMessages.Count > 2000)
            {
                LogMessages.RemoveAt(0);
            }
        });
    }

    private void _fileProcessingService_Progress(object? sender, ProcessingProgressEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (e.ProcessedBytes >= e.TotalBytes)
            {
                CurrentProgress = $"Готово: {e.FileName}";
            }
            else
            {
                var percentage = e.TotalBytes > 0 ? (double)e.ProcessedBytes / e.TotalBytes : 0;
                CurrentProgress = $"В работе: {e.FileName} ({percentage:P0})";
            }
        });
    }

    private void _fileProcessingService_QueueChanged(object? sender, IEnumerable<TextCleanerJob> e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            QueuedFiles.Clear();
            foreach (var job in e)
            {
                QueuedFiles.Add(Path.GetFileName(job.SourceFilePath));
            }
        });
    }

    [RelayCommand]
    private void AddFiles()
    {
        // по-хорошему, нужно делать отдельный диалогсервис,
        // чтобы совсем отвязать вьюмодель от UI (вдруг она же будет использоваться не с WPF а с другим фреймворком)
        // но у нас тестовое задание и мы торопимся.
        var dialog = new OpenFileDialog
        {
            Multiselect = true,
            Title = "Выберите файлы для обработки"
        };

        if (dialog.ShowDialog() != true) return;


        foreach (var sourceFilePath in dialog.FileNames)
        {
            var job = new TextCleanerJob
            {
                SourceFilePath = sourceFilePath,
                TargetFilePath = Path.Combine(_config.TargetDir, Path.GetFileName(sourceFilePath)),
                MinWordLength = _config.MinWordLength,
                ItemsToRemove = _config.RemovePunctuation? PunctuationProvider.AllPunctuation.ToList() : []
            };
            _fileProcessingService.Enqueue(job);
        }
    }

    


    
}