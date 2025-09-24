namespace TextCleaner.WPF.Interfaces.Logging;

public interface IUiLogRelayService
{
    event Action<string>? LogReceived;
    void Relay(string message);
}