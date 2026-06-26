using DataConcentrator.Services;
using System.Windows;

namespace ScadaWPF.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Unesite username i lozinku.";
                return;
            }

            bool success = UserService.Instance.Login(username, password);
            if (success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                txtError.Text = "Pogrešan username ili lozinka.";
                txtPassword.Clear();
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}