using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCleaner.WPF.Interfaces.Logging;

namespace TextCleaner.WPF.Logging;

public class WpfLoggerProvider(IUiLogRelayService relayService) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new WpfLogger(relayService);
    public void Dispose() { }
}