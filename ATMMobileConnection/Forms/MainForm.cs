using ATMMobileConnection.Enums;
using ATMMobileConnection.Services;

namespace ATMMobileConnection.Forms;

public class MainForm : Form
{
    private readonly AtmService _atmService;
    private readonly Label _lblCardNumber;
    private readonly Label _lblStatus;
    private readonly Label _lblConnection;
    private readonly Panel _pnlConnection;
    private readonly Label _lblCardInserted;
    private readonly Label _lblAuthorized;
    private readonly Label _lblAtmMessage;
    private readonly Label _lblBalanceValue;
    private readonly TextBox _txtAmount;
    private readonly ListBox _lstLog;

    public MainForm(AtmService atmService)
    {
        _atmService = atmService;

        Text = "Банкомат - Главное окно";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        ClientSize = new Size(820, 520);

        var groupUser = new GroupBox
        {
            Text = "Данные клиента",
            Location = new Point(20, 20),
            Size = new Size(370, 110)
        };

        _lblCardNumber = new Label { AutoSize = true, Location = new Point(20, 30) };
        _lblStatus = new Label { AutoSize = true, Location = new Point(20, 60) };
        _lblBalanceValue = new Label
        {
            AutoSize = true,
            Location = new Point(20, 85),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Text = "Баланс: не запрошен"
        };
        groupUser.Controls.AddRange(new Control[] { _lblCardNumber, _lblStatus, _lblBalanceValue });

        var groupConnection = new GroupBox
        {
            Text = "Состояние связи",
            Location = new Point(410, 20),
            Size = new Size(380, 110)
        };

        var lblConnectionTitle = new Label
        {
            Text = "Связь с банком:",
            AutoSize = true,
            Location = new Point(20, 35)
        };
        _pnlConnection = new Panel { Size = new Size(24, 24), Location = new Point(140, 30) };
        _lblConnection = new Label { AutoSize = true, Location = new Point(175, 35) };

        var btnToggleConnection = new Button
        {
            Text = "Изменить связь",
            Location = new Point(20, 70),
            Size = new Size(140, 30)
        };
        btnToggleConnection.Click += BtnToggleConnection_Click;

        groupConnection.Controls.AddRange(new Control[]
        {
            lblConnectionTitle, _pnlConnection, _lblConnection, btnToggleConnection
        });

        var groupOperations = new GroupBox
        {
            Text = "Операции",
            Location = new Point(20, 150),
            Size = new Size(370, 250)
        };

        var lblAmount = new Label
        {
            Text = "Сумма:",
            AutoSize = true,
            Location = new Point(20, 35)
        };
        _txtAmount = new TextBox
        {
            Location = new Point(90, 31),
            Width = 180
        };

        var btn500 = new Button { Text = "500", Location = new Point(20, 70), Size = new Size(70, 30) };
        var btn1000 = new Button { Text = "1000", Location = new Point(100, 70), Size = new Size(70, 30) };
        var btn5000 = new Button { Text = "5000", Location = new Point(180, 70), Size = new Size(70, 30) };

        btn500.Click += (_, _) => _txtAmount.Text = "500";
        btn1000.Click += (_, _) => _txtAmount.Text = "1000";
        btn5000.Click += (_, _) => _txtAmount.Text = "5000";

        var btnBalance = new Button
        {
            Text = "Проверить баланс",
            Location = new Point(20, 120),
            Size = new Size(150, 35)
        };
        btnBalance.Click += BtnBalance_Click;

        var btnWithdraw = new Button
        {
            Text = "Снять наличные",
            Location = new Point(190, 120),
            Size = new Size(150, 35)
        };
        btnWithdraw.Click += BtnWithdraw_Click;

        var btnDeposit = new Button
        {
            Text = "Пополнить счет",
            Location = new Point(20, 170),
            Size = new Size(150, 35)
        };
        btnDeposit.Click += BtnDeposit_Click;

        var btnHistory = new Button
        {
            Text = "История операций",
            Location = new Point(190, 170),
            Size = new Size(150, 35)
        };
        btnHistory.Click += BtnHistory_Click;

        var btnLogout = new Button
        {
            Text = "Выход из аккаунта",
            Location = new Point(105, 215),
            Size = new Size(150, 35)
        };
        btnLogout.Click += BtnLogout_Click;

        groupOperations.Controls.AddRange(new Control[]
        {
            lblAmount, _txtAmount, btn500, btn1000, btn5000,
            btnBalance, btnWithdraw, btnDeposit, btnHistory, btnLogout
        });

        var groupState = new GroupBox
        {
            Text = "Состояние банкомата",
            Location = new Point(410, 150),
            Size = new Size(380, 120)
        };

        _lblCardInserted = new Label { AutoSize = true, Location = new Point(20, 30) };
        _lblAuthorized = new Label { AutoSize = true, Location = new Point(20, 55) };
        _lblAtmMessage = new Label { AutoSize = false, Location = new Point(20, 80), Size = new Size(330, 30) };

        groupState.Controls.AddRange(new Control[] { _lblCardInserted, _lblAuthorized, _lblAtmMessage });

        var groupLog = new GroupBox
        {
            Text = "Журнал сообщений",
            Location = new Point(410, 290),
            Size = new Size(380, 190)
        };

        _lstLog = new ListBox
        {
            Dock = DockStyle.Fill
        };
        groupLog.Controls.Add(_lstLog);

        Controls.AddRange(new Control[]
        {
            groupUser, groupConnection, groupOperations, groupState, groupLog
        });

        UpdateView();
        AddLog("Пользователь вошел в систему.");
    }

    private void BtnToggleConnection_Click(object? sender, EventArgs e)
    {
        _atmService.ToggleConnection();
        UpdateView();
        AddLog(_atmService.State.CurrentMessage);
        MessageBox.Show(_atmService.State.CurrentMessage, "Состояние связи", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnBalance_Click(object? sender, EventArgs e)
    {
        if (_atmService.CheckBalance(out var balance, out var message))
        {
            _lblBalanceValue.Text = $"Баланс: {balance:F2} руб.";
            AddLog(message);
            UpdateView();
            return;
        }

        ShowOperationError(message);
    }

    private void BtnWithdraw_Click(object? sender, EventArgs e)
    {
        if (!TryParseAmount(out var amount))
        {
            return;
        }

        if (_atmService.Withdraw(amount, out var message))
        {
            AddLog(message);
            RefreshBalanceText();
            UpdateView();
            MessageBox.Show(message, "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ShowOperationError(message);
    }

    private void BtnDeposit_Click(object? sender, EventArgs e)
    {
        if (!TryParseAmount(out var amount))
        {
            return;
        }

        if (_atmService.Deposit(amount, out var message))
        {
            AddLog(message);
            RefreshBalanceText();
            UpdateView();
            MessageBox.Show(message, "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ShowOperationError(message);
    }

    private void BtnHistory_Click(object? sender, EventArgs e)
    {
        using var historyForm = new HistoryForm(_atmService.GetHistory());
        historyForm.ShowDialog(this);
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        _atmService.Logout();
        Close();
    }

    private bool TryParseAmount(out decimal amount)
    {
        if (!decimal.TryParse(_txtAmount.Text.Trim(), out amount) || amount <= 0)
        {
            amount = 0;
            MessageBox.Show("Введите корректную сумму.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private void ShowOperationError(string message)
    {
        AddLog(message);
        UpdateView();
        MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void RefreshBalanceText()
    {
        if (_atmService.CurrentCard != null)
        {
            _lblBalanceValue.Text = $"Баланс: {_atmService.CurrentCard.Account.Balance:F2} руб.";
        }
    }

    private void UpdateView()
    {
        var cardNumber = _atmService.CurrentCard?.CardNumber ?? "-";
        _lblCardNumber.Text = $"Карта: {cardNumber}";
        _lblStatus.Text = _atmService.State.IsAuthorized ? "Статус: авторизован" : "Статус: не авторизован";
        _lblCardInserted.Text = $"Карта вставлена: {(_atmService.State.IsCardInserted ? "Да" : "Нет")}";
        _lblAuthorized.Text = $"Авторизация: {(_atmService.State.IsAuthorized ? "Выполнена" : "Нет")}";
        _lblAtmMessage.Text = $"Сообщение: {_atmService.State.CurrentMessage}";

        var connected = _atmService.State.ConnectionStatus == ConnectionStatus.Connected;
        _pnlConnection.BackColor = connected ? Color.LimeGreen : Color.Red;
        _lblConnection.Text = connected ? "Связь есть" : "Связи нет";
    }

    private void AddLog(string message)
    {
        _lstLog.Items.Insert(0, $"{DateTime.Now:HH:mm:ss} - {message}");
    }
}
