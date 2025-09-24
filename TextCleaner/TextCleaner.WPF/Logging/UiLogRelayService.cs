using TextCleaner.WPF.Interfaces.Logging;

namespace TextCleaner.WPF.Logging;

public class UiLogRelayService : IUiLogRelayService
{
    public event Action<string>? LogReceived;
    public void Relay(string message) => LogReceived?.Invoke(message);
}