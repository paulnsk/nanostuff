using Microsoft.Extensions.Logging;
using TextCleaner.WPF.Interfaces.Logging;

namespace TextCleaner.WPF.Logging;

public class WpfLogger(IUiLogRelayService relayService) : ILogger
{

    public bool IsEnabled(LogLevel logLevel) => true; 
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null!;
    
    /// <summary>
    /// Отдает логгируемое сообщение рилей-сервису, который превращает его в ивент, на который по желанию может подписаться юай
    /// </summary>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = $"[{logLevel}] {formatter(state, exception)}";
        relayService.Relay(message);
    }
}