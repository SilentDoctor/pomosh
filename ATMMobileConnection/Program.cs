using System.Globalization;
using ATMMobileConnection.Data;
using ATMMobileConnection.Forms;
using ATMMobileConnection.Services;

namespace ATMMobileConnection;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");

        var database = new FakeDatabase();
        var authService = new AuthService(database);
        var bankService = new BankService();
        var connectionService = new ConnectionService();
        var atmService = new AtmService(authService, bankService, connectionService);

        Application.Run(new LoginForm(atmService));
    }
}
