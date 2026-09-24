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
    public partial class WorkerWindow : System.Windows.Window
    {
        public WorkerWindow()
        {
            InitializeComponent();
            LoadEmployeeInfo();
        }

        private void LoadEmployeeInfo()
        {
            try
            {
                var employee = App.Context.Employees.FirstOrDefault(p => p.IdUser == App.CurrentUser.IdUser);

                if (employee != null)
                {
                    tbFullName.Text = employee.LastName + " " + employee.FirstName + " " + employee.MiddleName;
                    tbPhone.Text = employee.Phone;
                    tbEmail.Text = employee.Email;

                    if (employee.BirthDate != null)
                    {
                        tbBirthDate.Text = Convert.ToDateTime(employee.BirthDate).ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        tbBirthDate.Text = "Не указана";
                    }
                }
                else
                {
                    MessageBox.Show("Данные сотрудника не найдены.");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки данных сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            CatalogWindow catalogWindow = new CatalogWindow();
            catalogWindow.Show();
            this.Close();
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            CartWindow cartWindow = new CartWindow();
            cartWindow.Show();
            this.Close();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {
            WorkSchedule window = new WorkSchedule();
            window.Show();
            this.Close();
        }
    }
}