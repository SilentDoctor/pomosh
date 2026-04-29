using ATMMobileConnection.Enums;

namespace ATMMobileConnection.Models;

public class AtmState
{
    public bool IsCardInserted { get; set; }
    public bool IsAuthorized { get; set; }
    public ConnectionStatus ConnectionStatus { get; set; }
    public string CurrentMessage { get; set; } = "Банкомат готов к работе";
}
