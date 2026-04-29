using ATMMobileConnection.Enums;
using ATMMobileConnection.Services;

namespace ATMMobileConnection.Forms;

public class LoginForm : Form
{
    private readonly AtmService _atmService;
    private readonly TextBox _txtCardNumber;
    private readonly TextBox _txtPinCode;
    private readonly Label _lblConnectionStatus;
    private readonly Panel _pnlConnectionIndicator;
    private readonly Label _lblHint;

    public LoginForm(AtmService atmService)
    {
        _atmService = atmService;
        Text = "Банкомат - Авторизация";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        ClientSize = new Size(420, 320);

        var lblTitle = new Label
        {
            Text = "Авторизация в банкомате",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(95, 20)
        };

        var lblCardNumber = new Label
        {
            Text = "Номер карты:",
            AutoSize = true,
            Location = new Point(40, 80)
        };

        _txtCardNumber = new TextBox
        {
            Location = new Point(160, 76),
            Width = 180,
            MaxLength = 16,
            Text = "1111222233334444"
        };

        var lblPinCode = new Label
        {
            Text = "PIN-код:",
            AutoSize = true,
            Location = new Point(40, 120)
        };

        _txtPinCode = new TextBox
        {
            Location = new Point(160, 116),
            Width = 180,
            MaxLength = 4,
            Text = "1234",
            UseSystemPasswordChar = true
        };

        var lblConnection = new Label
        {
            Text = "Связь с банком:",
            AutoSize = true,
            Location = new Point(40, 165)
        };

        _pnlConnectionIndicator = new Panel
        {
            Size = new Size(20, 20),
            Location = new Point(160, 162)
        };

        _lblConnectionStatus = new Label
        {
            AutoSize = true,
            Location = new Point(190, 165)
        };

        _lblHint = new Label
        {
            Text = "Тестовая карта: 1111222233334444 / PIN: 1234",
            AutoSize = true,
            ForeColor = Color.DimGray,
            Location = new Point(40, 205)
        };

        var btnLogin = new Button
        {
            Text = "Войти",
            Location = new Point(90, 245),
            Size = new Size(100, 35)
        };
        btnLogin.Click += BtnLogin_Click;

        var btnToggleConnection = new Button
        {
            Text = "Изменить связь",
            Location = new Point(210, 245),
            Size = new Size(120, 35)
        };
        btnToggleConnection.Click += BtnToggleConnection_Click;

        Controls.AddRange(new Control[]
        {
            lblTitle, lblCardNumber, _txtCardNumber, lblPinCode, _txtPinCode,
            lblConnection, _pnlConnectionIndicator, _lblConnectionStatus, _lblHint,
            btnLogin, btnToggleConnection
        });

        UpdateConnectionView();
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        if (_atmService.Authorize(_txtCardNumber.Text.Trim(), _txtPinCode.Text.Trim(), out var message))
        {
            Hide();
            using var mainForm = new MainForm(_atmService);
            mainForm.ShowDialog();

            if (!_atmService.State.IsAuthorized)
            {
                _txtPinCode.Clear();
                Show();
                UpdateConnectionView();
                return;
            }

            Close();
            return;
        }

        UpdateConnectionView();
        MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void BtnToggleConnection_Click(object? sender, EventArgs e)
    {
        _atmService.ToggleConnection();
        UpdateConnectionView();
        MessageBox.Show(_atmService.State.CurrentMessage, "Состояние связи", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void UpdateConnectionView()
    {
        var isConnected = _atmService.State.ConnectionStatus == ConnectionStatus.Connected;
        _pnlConnectionIndicator.BackColor = isConnected ? Color.LimeGreen : Color.Red;
        _lblConnectionStatus.Text = isConnected ? "Связь есть" : "Связи нет";
    }
}
