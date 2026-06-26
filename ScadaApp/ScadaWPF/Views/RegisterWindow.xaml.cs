using DataConcentrator.Models;
using DataConcentrator.Services;
using System.Windows;
using System.Windows.Controls;

namespace ScadaWPF.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;
            string confirm = txtConfirmPassword.Password;
            string roleStr = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Popunite sva polja.";
                return;
            }

            if (password != confirm)
            {
                txtError.Text = "Lozinke se ne poklapaju.";
                return;
            }

            if (!UserService.Instance.ValidatePassword(password))
            {
                txtError.Text = "Lozinka mora imati min 15 karaktera, 1 veliko, 1 malo, 1 specijalni znak.";
                return;
            }

            if (roleStr == null)
            {
                txtError.Text = "Izaberi ulogu.";
                return;
            }

            UserRole role = (UserRole)System.Enum.Parse(typeof(UserRole), roleStr);
            bool success = UserService.Instance.Register(username, password, role);

            if (success)
            {
                MessageBox.Show("Uspešna registracija!");
                Close();
            }
            else
            {
                txtError.Text = "Username već postoji ili lozinka nije jedinstvena.";
            }
        }
    }
}