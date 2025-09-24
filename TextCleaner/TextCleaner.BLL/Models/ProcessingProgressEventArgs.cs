using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCleaner.BLL.Models;

public record ProcessingProgressEventArgs(
    string FileName,
    long TotalBytes,
    long ProcessedBytes
);