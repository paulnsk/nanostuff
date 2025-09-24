using CommunityToolkit.Mvvm.ComponentModel;
using NanoAtm.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAtm.ViewModels;


public partial class AtmViewModel : ObservableObject
{
    [ObservableProperty]
    private long _totalBalance;

    public ObservableCollection<CassetteViewModel> Cassettes { get; } = [];

    public AtmViewModel()
    {
        //Инициализируем банкомат с полными кассетами
        var denominations = (Denomination[])Enum.GetValues(typeof(Denomination));
        foreach (var den in denominations.OrderByDescending(d => d))
        {
            var cassette = new CassetteViewModel(den, CassetteViewModel.MaxCapacity); 
            cassette.PropertyChanged += (s, e) => RecalculateTotalBalance();
            Cassettes.Add(cassette);
        }
        RecalculateTotalBalance();
    }
    
    public (CashBundleViewModel accepted, CashBundleViewModel returned) Deposit(CashBundleViewModel depositBundle)
    {
        var accepted = new CashBundleViewModel();
        var returned = new CashBundleViewModel();

        // Раскладываем пачку по кассетам
        foreach (var entry in depositBundle.Notes.OrderByDescending(n => n.Denomination))
        {
            var cassette = Cassettes.FirstOrDefault(c => c.Denomination == entry.Denomination);
            if (cassette == null)
            {
                returned.Add(entry.Denomination, entry.Count);
                continue;
            }

            var spaceAvailable = cassette.Capacity - cassette.Count;
            var toAccept = Math.Min(entry.Count, spaceAvailable);
            var toReturn = entry.Count - toAccept;

            if (toAccept > 0)
            {
                cassette.Count += toAccept;
                accepted.Add(entry.Denomination, toAccept);
            }
            if (toReturn > 0)
            {
                returned.Add(entry.Denomination, toReturn);
            }
        }

        //что-то влезло, что-то не влезло, возвращаем.
        return (accepted, returned);
    }

    public CashBundleViewModel? Withdraw(int amount, bool preferLargeBills)
    {
        if (amount <= 0 || amount > TotalBalance) return null;

        // решаем, в каком порядке будем выдавать купюры - покрупней или помельче
        var orderedCassettes = preferLargeBills
            ? Cassettes.OrderByDescending(c => c.Denomination)
            : Cassettes.OrderBy(c => c.Denomination);

        var resultBundle = new CashBundleViewModel();
        var remainingAmount = amount;

        var сassetteCounts = Cassettes.ToDictionary(c => c.Denomination, c => c.Count);

        // Для варианта "покрупней" этого алгоритма достаточно - он либо выдаст нужную сумму либо ВСЕХ денег банкомата не хватит:
        foreach (var cassette in orderedCassettes)
        {
            if (remainingAmount == 0) break;
            var denValue = (int)cassette.Denomination;
            if (denValue > remainingAmount) continue;

            var notesNeeded = remainingAmount / denValue;
            var notesToDispense = Math.Min(notesNeeded, сassetteCounts[cassette.Denomination]);

            if (notesToDispense > 0)
            {
                resultBundle.Add(cassette.Denomination, notesToDispense);
                remainingAmount -= notesToDispense * denValue;
                сassetteCounts[cassette.Denomination] -= notesToDispense;
            }
        }
        

        if (remainingAmount == 0)
        {
            // Нашли вариант как выдать требуемую сумму, вытягиваем бумажки из кассет
            foreach (var entry in resultBundle.Notes)
            {
                var cassette = Cassettes.First(c => c.Denomination == entry.Denomination);
                cassette.Count -= entry.Count;
            }
            return resultBundle;
        }

        return null; // Не нашли варианта
    }

    /// <summary>
    /// Пользователь не успел вынуть деньги из лотка, сжираем их обратно и раскладываем по коробочкам
    /// </summary>
    /// <param name="returnedBundle"></param>
    public void ReturnDispensedMoney(CashBundleViewModel returnedBundle)
    {
        foreach (var entry in returnedBundle.Notes)
        {
            var cassette = Cassettes.FirstOrDefault(c => c.Denomination == entry.Denomination);
            if (cassette != null)
            {
                cassette.Count += entry.Count;
            }
        }
    }

    private void RecalculateTotalBalance()
    {
        TotalBalance = Cassettes.Sum(c => (long)c.Denomination * c.Count);
    }
}