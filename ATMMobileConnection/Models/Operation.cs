using ATMMobileConnection.Enums;

namespace ATMMobileConnection.Models;

public class Operation
{
    public DateTime Date { get; set; }
    public OperationType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
}
