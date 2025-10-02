using System.Globalization;
using System.Windows;

using System.Windows.Data;

namespace NanoAtm.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }


    /// <summary>
    /// Конвертер для визибилити оверлейного грида, который поверх контента главного окна и в котором мы показываем "диалоги"
    /// </summary>
    public class ObjectToVisibilityConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}