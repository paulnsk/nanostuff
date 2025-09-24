using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TextCleaner.WPF.Utilities;

public static class DiHelpers
{
    private static T GetJustRegisteredService<T>(this IServiceCollection services) where T : class
    {
        using var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<T>();
    }

    public static void AddConfigOptions<T>(this IServiceCollection services, Action<T>? configAction = null) where T : class
    {
        var optionsBuilder = services.AddOptions<T>().BindConfiguration(typeof(T).Name).ValidateDataAnnotations();
        if (configAction != null) services.Configure(configAction);
    }

    /// <summary>
    /// CAREFUL! BuildServiceProvider may cause your singletons to become multipletons.
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static TConfig GetJustRegisteredConfig<TConfig>(this IServiceCollection services) where TConfig : class
    {
        return services.GetJustRegisteredService<IOptions<TConfig>>().Value;
    }
}