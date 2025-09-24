using System.Text;
using TextCleaner.BLL.Utilities;

namespace TestCleaner.BLL.Tests;

[TestClass]
public class BlockStreamReaderTests
{
    [TestMethod]
    public void ReadBlocks_ShouldCorrectlySplitTextByWords()
    {
        // Arrange
        var text = "Это первая часть текста длиной 40 букв. А это весь остальной текст   с пробелами 12345 цифрами и матерными %@(*^%♣♦ знаками";
        var textBytes = Encoding.UTF8.GetBytes(text);
        using var memoryStream = new MemoryStream(textBytes);
        var reader = new BlockStreamReader(memoryStream, 40); // Маленький буфер для теста

        // Act
        var blocks = reader.ReadBlocks(CancellationToken.None).ToList();

        // Assert
        Assert.IsTrue(blocks.Count > 1);
        Assert.AreEqual(text.Replace(" ", ""), string.Join("", blocks).Replace(" ", "")); // весь контент на месте
        Assert.AreEqual("Это первая часть текста длиной 40 букв. ", blocks[0]);
    }
}