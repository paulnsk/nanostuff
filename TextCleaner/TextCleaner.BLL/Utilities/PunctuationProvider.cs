namespace TextCleaner.BLL.Utilities;

public static class PunctuationProvider
{
    public static readonly IReadOnlyList<string> AllPunctuation;
    
    static PunctuationProvider()
    {
        AllPunctuation = Enumerable
            .Range(0, char.MaxValue + 1)
            .Select(i => (char)i)
            .Where(char.IsPunctuation)
            .Select(c => c.ToString())
            .ToList(); 
    }
}