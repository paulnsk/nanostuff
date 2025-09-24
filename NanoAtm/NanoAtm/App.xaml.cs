
using NanoAtm.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using NanoAtm.ViewModels;

namespace NanoAtm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var atmViewModel = new AtmViewModel();
            var mainViewModel = new MainViewModel(atmViewModel);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            var serviceWindow = new ServiceWindow
            {
                DataContext = atmViewModel
            };

            // закрываем приложение по закрытию любого окна. По главному само закроется, по сервисному - этой командой:
            serviceWindow.Closing += (s, a) => Application.Current.Shutdown();


            //Раскладываем окна по экрану (хотя лучше бы главное фулскрин, а сервисное на второй монитор, но вдруг нет второго монитора)

            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            mainWindow.Left = 0;
            mainWindow.Top = 0;
            mainWindow.Width = screenWidth * 0.65; // Главное окно 65% ширины и всю высоту
            mainWindow.Height = screenHeight;

            serviceWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            serviceWindow.Left = mainWindow.Width;
            serviceWindow.Top = 0;
            serviceWindow.Width = screenWidth * 0.35; // Сервисное - оставшиеся 35%
            serviceWindow.Height = 800; 

            mainWindow.Show();
            serviceWindow.Show();
        }
    }

}
