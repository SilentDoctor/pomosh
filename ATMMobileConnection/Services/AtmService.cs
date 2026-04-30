using ATMMobileConnection.Enums;
using ATMMobileConnection.Models;

namespace ATMMobileConnection.Services;

public class AtmService
{
    private readonly AuthService _authService;
    private readonly BankService _bankService;
    private readonly ConnectionService _connectionService;

    public AtmService(AuthService authService, BankService bankService, ConnectionService connectionService)
    {
        _authService = authService;
        _bankService = bankService;
        _connectionService = connectionService;
        State = new AtmState
        {
            ConnectionStatus = _connectionService.CurrentStatus
        };
    }

    public AtmState State { get; }
    public BankCard? CurrentCard { get; private set; }

    public bool Authorize(string cardNumber, string pinCode, out string message)
    {
        SyncConnectionState();

        if (!_connectionService.IsConnected())
        {
            message = "Нет связи с банком. Авторизация невозможна.";
            State.CurrentMessage = message;
            return false;
        }

        var card = _authService.Login(cardNumber, pinCode);
        if (card == null)
        {
            message = "Неверный номер карты или PIN-код.";
            State.CurrentMessage = message;
            return false;
        }

        CurrentCard = card;
        State.IsCardInserted = true;
        State.IsAuthorized = true;
        State.CurrentMessage = "Авторизация выполнена успешно.";
        _bankService.AddOperation(card.Account, OperationType.Login, 0, "Вход в систему", true);
        message = State.CurrentMessage;
        return true;
    }

    public void Logout()
    {
        if (CurrentCard != null)
        {
            _bankService.AddOperation(CurrentCard.Account, OperationType.Logout, 0, "Выход из системы", true);
        }

        CurrentCard = null;
        State.IsCardInserted = false;
        State.IsAuthorized = false;
        State.CurrentMessage = "Выполнен выход из системы.";
        SyncConnectionState();
    }

    public bool CheckBalance(out decimal balance, out string message)
    {
        balance = 0;
        if (!CanProcessOperation(out message))
        {
            return false;
        }

        balance = _bankService.GetBalance(CurrentCard!.Account);
        _bankService.AddOperation(CurrentCard.Account, OperationType.BalanceCheck, 0, "Проверка баланса", true);
        State.CurrentMessage = $"Текущий баланс: {balance:F2} руб.";
        message = State.CurrentMessage;
        return true;
    }

    public bool Withdraw(decimal amount, out string message)
    {
        if (!CanProcessOperation(out message))
        {
            return false;
        }

        var success = _bankService.Withdraw(CurrentCard!.Account, amount, out message);
        if (!success)
        {
            _bankService.AddOperation(CurrentCard.Account, OperationType.Error, amount, message, false);
            State.CurrentMessage = message;
            return false;
        }

        State.CurrentMessage = message;
        return true;
    }

    public bool Deposit(decimal amount, out string message)
    {
        if (!CanProcessOperation(out message))
        {
            return false;
        }
 
        var success = _bankService.Deposit(CurrentCard!.Account, amount, out message);
        if (!success)
        {
            _bankService.AddOperation(CurrentCard.Account, OperationType.Error, amount, message, false);
            State.CurrentMessage = message;
            return false;
        }
 
        State.CurrentMessage = message;
        return true;
    }

    public bool TopUpPhone(string phoneNumber, decimal amount, out string message)
    {
        if (!CanProcessOperation(out message))
        {
            return false;
        }

        var success = _bankService.TopUpPhone(CurrentCard!.Account, phoneNumber, amount, out message);
        if (!success)
        {
            _bankService.AddOperation(CurrentCard.Account, OperationType.Error, amount, $"Пополнение номера {phoneNumber}: {message}", false);
            State.CurrentMessage = message;
            return false;
        }

        State.CurrentMessage = message;
        return true;
    }


    public List<Operation> GetHistory()
    {
        if (CurrentCard == null)
        {
            return new List<Operation>();
        }

        return _bankService.GetHistory(CurrentCard.Account);
    }

    public void ToggleConnection()
    {
        _connectionService.ToggleConnection();
        SyncConnectionState();
        State.CurrentMessage = _connectionService.IsConnected()
            ? "Связь с банком восстановлена."
            : "Связь с банком потеряна.";
    }

    private bool CanProcessOperation(out string message)
    {
        SyncConnectionState();

        if (!State.IsAuthorized || CurrentCard == null)
        {
            message = "Пользователь не авторизован.";
            State.CurrentMessage = message;
            return false;
        }

        if (!_connectionService.IsConnected())
        {
            message = "Нет связи с банком. Операция невозможна.";
            State.CurrentMessage = message;
            _bankService.AddOperation(CurrentCard.Account, OperationType.Error, 0, message, false);
            return false;
        }

        message = string.Empty;
        return true;
    }

    private void SyncConnectionState()
    {
        State.ConnectionStatus = _connectionService.CurrentStatus;
    }
}
