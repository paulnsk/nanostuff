using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace TextCleaner.BLL.Services;


/// <summary>
/// Чистит один фрагмент текста, удаляя короткие слова и подстроки (напр. знаки препинания) из списка
/// </summary>
/// <param name="logger"></param>
public class TextCleaner(ILogger<TextCleaner> logger)
{
    private readonly ILogger<TextCleaner> _logger = logger;

    // Regex компилируется один раз для производительности.
    // Ищет слово и ОДИН опциональный пробел (\s?)
    private static readonly Regex WordRegex = new(@"\b[^\s]+\b\s?", RegexOptions.Compiled);


    public string CleanText(string textBlock, int minWordLength, IEnumerable<string> itemsToRemove) 
    {
        // 1. Удаляем указанные подстроки (например, знаки препинания)
        var temp = itemsToRemove.Aggregate(
            textBlock,
            (current, itemToRemove) => current.Replace(itemToRemove, string.Empty)
        );

        // 2. Удаляем слова, которые короче или равны MinWordLength, и по одному пробелу за словом (если там пробел)
        return WordRegex.Replace(temp, match =>
        {
            // убираем возможный пробел для проверки длины
            var word = match.Value.TrimEnd();
            // Если слово короткое, удаляем весь match (слово + один пробел)
            return word.Length <= minWordLength ? "" : match.Value;
        });
    }
}