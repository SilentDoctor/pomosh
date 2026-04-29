namespace ATMMobileConnection.Models;

public class BankCard
{
    public string CardNumber { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public BankAccount Account { get; set; } = new();
}
