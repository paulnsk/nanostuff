using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCleaner.BLL.Models;
using TextCleaner.BLL.Services;

namespace TestCleaner.BLL.Tests;

[TestClass]
public class PersistentQueueStorageServiceTests
{
    private string? _tempFile;

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }

    [TestMethod]
    public void SaveAndLoad_ShouldPreserveQueueState()
    {
        // Arrange
        // Хак, чтобы подменить путь к файлу внутри сервиса
        _tempFile = Path.GetTempFileName();
        var service = new PersistentQueueStorageService();
        var fieldInfo = typeof(PersistentQueueStorageService).GetField("_storageFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fieldInfo.SetValue(service, _tempFile);

        var jobsToSave = new List<TextCleanerJob>
        {
            new() { SourceFilePath = "file1.txt" },
            new() { SourceFilePath = "file2.txt" }
        };

        // Act
        service.Save(jobsToSave);
        var loadedJobs = service.Load().ToList();

        // Assert
        Assert.AreEqual(2, loadedJobs.Count);
        Assert.AreEqual("file1.txt", loadedJobs[0].SourceFilePath);
    }
}