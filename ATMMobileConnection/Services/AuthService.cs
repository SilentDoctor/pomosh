using ATMMobileConnection.Data;
using ATMMobileConnection.Models;

namespace ATMMobileConnection.Services;

public class AuthService
{
    private readonly FakeDatabase _database;

    public AuthService(FakeDatabase database)
    {
        _database = database;
    }

    public BankCard? Login(string cardNumber, string pinCode)
    {
        var card = _database.GetCard(cardNumber);

        if (card == null || card.IsBlocked)
        {
            return null;
        }

        return card.PinCode == pinCode ? card : null;
    }
}
