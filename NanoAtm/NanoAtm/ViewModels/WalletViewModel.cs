using CommunityToolkit.Mvvm.Input;
using NanoAtm.Enums;

namespace NanoAtm.ViewModels;

/// <summary>
/// Юзерконтрол, символизирующий кошелек юзера, в котором он может порыться и достать купюры для внесения в банкомат
/// </summary>
/// <param name="onDeposit"></param>
/// <param name="onCancel"></param>
public partial class WalletViewModel(Action<CashBundleViewModel> onDeposit, Action onCancel) : DialogViewModelBase
{
    /// <summary>
    /// Собранная пачка
    /// </summary>
    public CashBundleViewModel Bundle { get; } = new();
    
    // ReSharper disable once UnusedMember.Global (Он юзается через байндинг)
    public Denomination[] AvailableDenominations { get; } = (Denomination[])Enum.GetValues(typeof(Denomination));

    /// <summary>
    /// Добавить бумажку в пачку
    /// </summary>
    [RelayCommand]
    private void AddNote(Denomination denomination)
    {
        Bundle.Add(denomination, 1);
    }

    /// <summary>
    /// Нажать кнопку "внести"
    /// </summary>
    [RelayCommand]
    private void Deposit()
    {
        if (Bundle.TotalAmount > 0)
        {
            onDeposit(Bundle);
        }
    }

    /// <summary>
    /// Передумали вносить
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        onCancel();
    }
}