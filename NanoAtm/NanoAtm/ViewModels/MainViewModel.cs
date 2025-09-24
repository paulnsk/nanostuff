using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAtm.ViewModels;

/// <summary>
/// Главное окно (то, которое видит юзер)
/// </summary>
/// <param name="atmViewModel"></param>
public partial class MainViewModel(AtmViewModel atmViewModel) : ObservableObject
{
    [ObservableProperty]
    private DialogViewModelBase? _currentDialog;

    [ObservableProperty]
    private string _withdrawAmountString = string.Empty;

    [ObservableProperty]
    private string _message = "Добро пожаловать!";

    [RelayCommand]
    private void ShowDepositDialog()
    {
        CurrentDialog = new WalletViewModel(
            onDeposit: bundle =>
            {
                var (accepted, returned) = atmViewModel.Deposit(bundle);
                if (returned.TotalAmount > 0)
                {
                    ShowMessage($"Принято: {accepted.TotalAmount} руб. Возвращено: {returned.TotalAmount} руб. (кассеты переполнены)");
                }
                else
                {
                    ShowMessage($"Операция успешна. Внесено: {accepted.TotalAmount} руб.");
                }
                CurrentDialog = null;
            },
            onCancel: () =>
            {
                CurrentDialog = null;
                ShowMessage("Операция отменена");
            }
        );
    }

    [RelayCommand]
    private void Withdraw(string preferLarge)
    {
        // К моменту нажатия этой кнопки юзер уже ввел сумму с экранной клавиатуры или с настоящей.

        if (!int.TryParse(WithdrawAmountString, out int amount) || amount <= 0)
        {
            ShowMessage("Введите корректную сумму");
            return;
        }

        var preferLargeBills = bool.Parse(preferLarge);

        var dispensedBundle = atmViewModel.Withdraw(amount, preferLargeBills);

        if (dispensedBundle == null)
        {
            ShowMessage("Невозможно выдать указанную сумму имеющимися купюрами");
            return;
        }

        WithdrawAmountString = string.Empty;
        CurrentDialog = new DispenseViewModel(
            bundle: dispensedBundle,
            onTake: _ =>
            {
                CurrentDialog = null;
                ShowMessage("Спасибо, что воспользовались нашим банкоматом!");
            },
            onTimeout: bundle =>
            {
                atmViewModel.ReturnDispensedMoney(bundle);
                CurrentDialog = null;
                ShowMessage("Вы не забрали деньги. Они возвращены в банкомат.");
            }
        );
    }

    private async void ShowMessage(string text)
    {
        Message = text;
        await Task.Delay(5000);
        if (Message == text)
        {
            Message = "Добро пожаловать!";
        }
    }

    /// <summary>
    /// Onclick для кнопок экранной клавиатуры
    /// </summary>
    /// <param name="value"></param>
    [RelayCommand]
    private void AppendToWithdrawAmount(string value)
    {
        if (value == "BACK")
        {
            if (WithdrawAmountString.Length > 0)
            {
                WithdrawAmountString = WithdrawAmountString.Substring(0, WithdrawAmountString.Length - 1);
            }
        }
        else
        {
            if (string.IsNullOrEmpty(WithdrawAmountString) && (value == "0" || value == "00"))
            {
                return; // Не даем ввести нули в пустое поле
            }
            WithdrawAmountString += value;
            
        }
    }

}