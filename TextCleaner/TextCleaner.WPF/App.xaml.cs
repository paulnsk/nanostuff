using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TextCleaner.BLL.Interfaces;
using TextCleaner.BLL.Setup;
using TextCleaner.WPF.Logging;
using TextCleaner.WPF.Models;
using TextCleaner.WPF.Utilities;
using TextCleaner.WPF.ViewModels;

namespace TextCleaner.WPF
{

    
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    // Добавляем наш кустомный недо-логгер
                    logging.AddWpfLogger();
                })
                .ConfigureServices((context, services) =>
                {
                    
                    services.AddTextCleaner();
                    services.AddConfigOptions<TextCleanerConfig>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }


        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await _host.StartAsync();

                // сначала создаем юай, чтоб он подписался на какие ему надо ивенты
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();

                // Запускаем фоновый сервис обработки
                var fileProcessingService = _host.Services.GetRequiredService<IFileProcessingService>();
                fileProcessingService.Start();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                //ловим эксепшен тут, т.к. иначе он потеряется из-за асинк войд
                MessageBox.Show($"Что-то фундаментально сломалось: {ex}");
            }
        }


        protected override async void OnExit(ExitEventArgs e)
        {
            //лишним не будет
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }
    }

}
