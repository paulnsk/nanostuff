namespace TextCleaner.WPF.Models;

public class TextCleanerConfig
{
    public string TargetDir { get; set; } = string.Empty;
    public int MinWordLength { get; set; } = 0;
    public bool RemovePunctuation { get; set; } = true;
}