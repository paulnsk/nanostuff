using Microsoft.Extensions.Logging.Abstractions;

namespace TestCleaner.BLL.Tests;

[TestClass]
public class TextCleanerTests
{
    private readonly TextCleaner.BLL.Services.TextCleaner _textCleaner = new(new NullLogger<TextCleaner.BLL.Services.TextCleaner>());

    [TestMethod]
    public void CleanText_ShouldRemoveShortWords()
    {
        // Arrange
        var text = " Это тестовый текст для  проверки";
        var itemsToRemove = new List<string> { "," };
        var minLength = 4;
        var expected = " тестовый текст  проверки";

        // Act
        var result = _textCleaner.CleanText(text, minLength, itemsToRemove);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void CleanText_ShouldRemoveSpecifiedItems()
    {
        // Arrange
        var text = "Текст, с знаками! препинания.";
        var itemsToRemove = new List<string> { ",", "!", "." };
        var minLength = 0;
        var expected = "Текст с знаками препинания";

        // Act
        var result = _textCleaner.CleanText(text, minLength, itemsToRemove);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void CleanText_ShouldPreserveMultipleSpaces()
    {
        // Arrange
        var text = "Слово1    Слово2  Слово3";
        var itemsToRemove = new List<string>();
        var minLength = 0;
        var expected = "Слово1    Слово2  Слово3";

        // Act
        var result = _textCleaner.CleanText(text, minLength, itemsToRemove);

        // Assert
        Assert.AreEqual(expected, result);
    }
}