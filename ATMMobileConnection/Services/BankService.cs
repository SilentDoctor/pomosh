using ATMMobileConnection.Enums;
using ATMMobileConnection.Models;

namespace ATMMobileConnection.Services;

public class BankService
{
    public decimal GetBalance(BankAccount account)
    {
        return account.Balance;
    }

    public bool Withdraw(BankAccount account, decimal amount, out string message)
    {
        if (amount <= 0)
        {
            message = "Сумма должна быть больше нуля.";
            return false;
        }

        if (amount % 100 != 0)
        {
            message = "Сумма должна быть кратна 100.";
            return false;
        }

        if (account.Balance < amount)
        {
            message = "Недостаточно средств на счете.";
            return false;
        }

        account.Balance -= amount;
        AddOperation(account, OperationType.Withdraw, amount, "Снятие наличных", true);
        message = $"Выдано {amount:F2} руб.";
        return true;
    }

    public bool Deposit(BankAccount account, decimal amount, out string message)
    {
        if (amount <= 0)
        {
            message = "Сумма должна быть больше нуля.";
            return false;
        }

        account.Balance += amount;
        AddOperation(account, OperationType.Deposit, amount, "Пополнение счета", true);
        message = $"Счет пополнен на {amount:F2} руб.";
        return true;
    }

    public List<Operation> GetHistory(BankAccount account)
    {
        return account.Operations
            .OrderByDescending(operation => operation.Date)
            .ToList();
    }

    public void AddOperation(BankAccount account, OperationType type, decimal amount, string description, bool isSuccessful)
    {
        account.Operations.Add(new Operation
        {
            Date = DateTime.Now,
            Type = type,
            Amount = amount,
            Description = description,
            IsSuccessful = isSuccessful
        });
    }
}
