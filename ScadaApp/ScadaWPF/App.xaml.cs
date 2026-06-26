using DataConcentrator.Services;
using ScadaWPF.Views;
using System.Windows;

namespace ScadaWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var login = new LoginWindow();
            var result = login.ShowDialog();

            if (result == true && UserService.Instance.CurrentUser != null)
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                var main = new MainWindow();
                this.MainWindow = main;
                main.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}