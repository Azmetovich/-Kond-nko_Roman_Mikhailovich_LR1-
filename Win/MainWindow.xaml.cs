using Pizza.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pizza
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TBoxLogin.Text) ||
                    string.IsNullOrWhiteSpace(PBoxPassword.Password))
                {
                    MessageBox.Show("Заполните все поля.");
                    return;
                }
                var currentUser = App.Context.Users.FirstOrDefault(p =>
                p.Login == TBoxLogin.Text &&
                p.Password == PBoxPassword.Password);
                if (currentUser != null)
                {
                    App.CurrentUser = currentUser;
                    if (currentUser.IdRole == 1)
                    {
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                        this.Close();
                    }
                    else if (currentUser.IdRole == 2)
                    {
                        WorkerWindow workerWindow = new WorkerWindow();
                        workerWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("У пользователя не найдена роль.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь с такими данными не найден.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}