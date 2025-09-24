using Microsoft.Extensions.Options;
using Moq;
using TextCleaner.BLL.Interfaces;
using TextCleaner.BLL.Models;
using TextCleaner.WPF.Interfaces.Logging;
using TextCleaner.WPF.Models;
using TextCleaner.WPF.ViewModels;

namespace TextCleaner.WPF.Tests;


[TestClass]
public class MainViewModelTests
{
    private Mock<IFileProcessingService>? _mockFileService;
    private Mock<IUiLogRelayService>? _mockLogRelay;
    private Mock<IOptions<TextCleanerConfig>>? _mockConfig;
    // ReSharper disable once NotAccessedField.Local (оно на самом деле юзается)
    private MainViewModel? _viewModel;

    [TestInitialize]
    public void Setup()
    {
        _mockFileService = new Mock<IFileProcessingService>();
        _mockLogRelay = new Mock<IUiLogRelayService>();
        _mockConfig = new Mock<IOptions<TextCleanerConfig>>();
        
        _mockConfig.Setup(c => c.Value).Returns(new TextCleanerConfig { TargetDir = "C:\\Target", MinWordLength = "3" });

        _viewModel = new MainViewModel(_mockFileService.Object, _mockLogRelay.Object, _mockConfig.Object);
    }

    [TestMethod]
    public void Constructor_ShouldSubscribeToEvents()
    {
        // Assert
        // Проверяем, что ViewModel подписалась на события сервисов
        _mockFileService!.VerifyAdd(s => s.QueueChanged += It.IsAny<EventHandler<IEnumerable<TextCleanerJob>>>(), Times.Once);
        _mockFileService!.VerifyAdd(s => s.Progress += It.IsAny<EventHandler<ProcessingProgressEventArgs>>(), Times.Once);
        _mockLogRelay!.VerifyAdd(s => s.LogReceived += It.IsAny<Action<string>>(), Times.Once);
    }

}