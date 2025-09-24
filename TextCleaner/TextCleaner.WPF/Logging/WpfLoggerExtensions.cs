using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using TextCleaner.WPF.Interfaces.Logging;

namespace TextCleaner.WPF.Logging;

public static class WpfLoggerExtensions
{
    public static ILoggingBuilder AddWpfLogger(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, WpfLoggerProvider>());
        builder.Services.AddSingleton<IUiLogRelayService, UiLogRelayService>();
        return builder;
    }
}