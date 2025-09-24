using CommunityToolkit.Mvvm.ComponentModel;
using NanoAtm.Enums;

namespace NanoAtm.ViewModels;

/// <summary>
/// Кассета для купюр определенного номинала внутри банкомата.
/// </summary>
public partial class CassetteViewModel(Denomination denomination, int initialCount) : ObservableObject
{
    public const int MaxCapacity = 10; // специально мало для тестирования

    [ObservableProperty]
    private Denomination _denomination = denomination;

    [ObservableProperty]
    private int _count = initialCount;

    [ObservableProperty] private int _capacity = MaxCapacity;
}