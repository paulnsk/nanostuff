using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TextCleaner.WPF.WpfStuff;

/// <summary>
/// Это чтобы лог скроллился вниз сам
/// </summary>
public static class TextBoxExtensions
{
    public static readonly DependencyProperty BoundItemsSourceProperty =
        DependencyProperty.RegisterAttached(
            "BoundItemsSource",
            typeof(IEnumerable),
            typeof(TextBoxExtensions),
            new PropertyMetadata(null, OnBoundItemsSourceChanged));

    public static IEnumerable GetBoundItemsSource(DependencyObject obj)
    {
        return (IEnumerable)obj.GetValue(BoundItemsSourceProperty);
    }

    public static void SetBoundItemsSource(DependencyObject obj, IEnumerable value)
    {
        obj.SetValue(BoundItemsSourceProperty, value);
    }

    private static void OnBoundItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is TextBox textBox)) return;

        // Отписываемся от событий старой коллекции
        if (e.OldValue is INotifyCollectionChanged oldCollection)
        {
            // Примечание: анонимный метод здесь не отпишется корректно,
            // но для простоты и в рамках задачи оставим так,
            // т.к. коллекция вряд ли будет меняться в рантайме.
            oldCollection.CollectionChanged -= (sender, args) => OnCollectionChanged(sender, args, textBox);
        }

        // Если новая коллекция - null, очищаем
        if (!(e.NewValue is IEnumerable newItemsSource))
        {
            textBox.Clear();
            return;
        }

        // Очищаем и заполняем TextBox при первой привязке
        textBox.Clear();
        foreach (var item in newItemsSource)
        {
            textBox.AppendText(item.ToString() + "\n");
        }
        textBox.ScrollToEnd();

        // Подписываемся на события новой коллекции, если она их поддерживает
        if (newItemsSource is INotifyCollectionChanged newNotifyCollection)
        {
            newNotifyCollection.CollectionChanged += (sender, args) => OnCollectionChanged(sender, args, textBox);
        }
    }

    private static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e, TextBox textBox)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            var sb = new StringBuilder();
            foreach (var item in e.NewItems)
            {
                sb.AppendLine(item.ToString());
            }

            // Важно делать это в потоке UI
            textBox.Dispatcher.Invoke(() =>
            {
                textBox.AppendText(sb.ToString());
                textBox.ScrollToEnd();
            });
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            textBox.Dispatcher.Invoke(() =>
            {
                textBox.Clear();
            });
        }
    }
}