using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCleaner.BLL.Interfaces;
using TextCleaner.BLL.Services;

namespace TextCleaner.BLL.Setup;

public static class TextCleanerServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует text cleaner сервисы
    /// </summary>
    public static IServiceCollection AddTextCleaner(this IServiceCollection services)
    {
        services.AddSingleton<IPersistentQueueStorageService, PersistentQueueStorageService>();
        services.AddSingleton<IFileProcessingService, FileProcessingService>();
        services.AddTransient<Services.TextCleaner>();
        return services;
    }
}