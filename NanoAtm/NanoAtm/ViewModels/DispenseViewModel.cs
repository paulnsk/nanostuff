using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NanoAtm.ViewModels;

/// <summary>
/// Диалог, показывающий пачку денег, торчащую из банкомата, которую можно забрать, или ее всосет по таймеру
/// </summary>
public partial class DispenseViewModel : DialogViewModelBase
{
    public CashBundleViewModel Bundle { get; }

    [ObservableProperty]
    private int _countdown = 30;

    private readonly DispatcherTimer _timer;
    private readonly Action<CashBundleViewModel> _onTake;
    private readonly Action<CashBundleViewModel> _onTimeout;

    public DispenseViewModel(CashBundleViewModel bundle, Action<CashBundleViewModel> onTake, Action<CashBundleViewModel> onTimeout)
    {
        Bundle = bundle;
        _onTake = onTake;
        _onTimeout = onTimeout;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        Countdown--;
        if (Countdown <= 0)
        {
            _timer.Stop();
            _onTimeout(Bundle);
        }
    }

    [RelayCommand]
    private void Take()
    {
        _timer.Stop();
        _onTake(Bundle);
    }
}