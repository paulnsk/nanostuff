using CommunityToolkit.Mvvm.ComponentModel;
using NanoAtm.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NanoAtm.ViewModels;


/// <summary>
/// Модель "пачки денег", используемая как для выдачи, так и для внесения. Пачка может быть набрана автоматически из кассет банкомата или руками из кошелька юзера
/// </summary>
public partial class CashBundleViewModel : ObservableObject
{
    public ObservableCollection<BanknoteEntry> Notes { get; } = [];

    [ObservableProperty]
    private long _totalAmount;

    public CashBundleViewModel()
    {
        // прокидываем пропертичейнджи от индивидуальных Notes чтобы при их изменении пересчитывался баланс всей пачки
        Notes.CollectionChanged += (s, e) =>
        {
            RecalculateTotal();
            if (e.NewItems != null)
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += BanknoteEntry_PropertyChanged;
            if (e.OldItems != null)
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= BanknoteEntry_PropertyChanged;
        };
    }

    private void BanknoteEntry_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BanknoteEntry.Count))
        {
            RecalculateTotal();
        }
    }

    /// <summary>
    /// Добавляем одну бумажку в пачку и обновляем коллекцию 
    /// </summary>
    public void Add(Denomination denomination, int count)
    {
        if (count <= 0) return;
        var entry = Notes.FirstOrDefault(n => n.Denomination == denomination);
        if (entry != null)
        {
            entry.Count += count;
        }
        else
        {
            Notes.Add(new BanknoteEntry(denomination, count));
        }
    }

    public void Clear()
    {
        Notes.Clear();
    }

    private void RecalculateTotal()
    {
        TotalAmount = Notes.Sum(n => (long)n.Denomination * n.Count);
    }

    /// <summary>
    /// Пачка денег одного номинала, например 5 купюр по 100 рублей
    /// </summary>
    public partial class BanknoteEntry(Denomination denomination, int count) : ObservableObject
    {
        [ObservableProperty]
        private Denomination _denomination = denomination;

        [ObservableProperty]
        private int _count = count;
    }
}