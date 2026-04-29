namespace ATMMobileConnection.Models;

public class BankAccount
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public List<Operation> Operations { get; } = new();
}
