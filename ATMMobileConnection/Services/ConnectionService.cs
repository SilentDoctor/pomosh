using ATMMobileConnection.Enums;

namespace ATMMobileConnection.Services;

public class ConnectionService
{
    public ConnectionStatus CurrentStatus { get; private set; } = ConnectionStatus.Connected;

    public bool IsConnected()
    {
        return CurrentStatus == ConnectionStatus.Connected;
    }

    public void SetConnectionStatus(ConnectionStatus status)
    {
        CurrentStatus = status;
    }

    public void ToggleConnection()
    {
        CurrentStatus = IsConnected() ? ConnectionStatus.Disconnected : ConnectionStatus.Connected;
    }
}
