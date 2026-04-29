using ATMMobileConnection.Models;

namespace ATMMobileConnection.Data;

public class FakeDatabase
{
    private readonly List<BankCard> _cards;

    public FakeDatabase()
    {
        _cards = CreateCards();
    }

    public List<BankCard> Cards => _cards;

    public BankCard? GetCard(string cardNumber)
    {
        return _cards.FirstOrDefault(card => card.CardNumber == cardNumber);
    }

    private static List<BankCard> CreateCards()
    {
        return new List<BankCard>
        {
            new()
            {
                CardNumber = "1111222233334444",
                PinCode = "1234",
                IsBlocked = false,
                Account = new BankAccount
                {
                    AccountNumber = "ACC-1001",
                    Balance = 15000m,
                }
            },
            new()
            {
                CardNumber = "5555666677778888",
                PinCode = "4321",
                IsBlocked = false,
                Account = new BankAccount
                {
                    AccountNumber = "ACC-2002",
                    Balance = 8200m,
                }
            }
        };
    }
}
