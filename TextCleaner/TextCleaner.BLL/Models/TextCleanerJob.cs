using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCleaner.BLL.Models;

public class TextCleanerJob
{
    public string SourceFilePath { get; set; } = string.Empty;
    public string TargetFilePath { get; set; } = string.Empty;
    public int MinWordLength { get; set; } = 0;
    public List<string> ItemsToRemove { get; set; } = [];
}