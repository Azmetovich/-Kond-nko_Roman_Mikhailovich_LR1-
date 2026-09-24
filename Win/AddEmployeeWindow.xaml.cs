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
    public partial class AddEmployeeWindow : System.Windows.Window
    {
        public AddEmployeeWindow()
        {
            InitializeComponent();
        }
        private string CheckErrors()
        {
            StringBuilder errorBuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(TBoxLastName.Text))
            {
                errorBuilder.AppendLine("Фамилия обязательна для заполнения;");
            }
            if (string.IsNullOrWhiteSpace(TBoxFirstName.Text))
            {
                errorBuilder.AppendLine("Имя обязательно для заполнения;");
            }
            if (string.IsNullOrWhiteSpace(TBoxPhone.Text))
            {
                errorBuilder.AppendLine("Телефон обязателен для заполнения;");
            }
            if (string.IsNullOrWhiteSpace(TBoxEmail.Text))
            {
                errorBuilder.AppendLine("Email обязателен для заполнения;");
            }
            if (DPBirthDate.SelectedDate == null)
            {
                errorBuilder.AppendLine("Дата рождения обязательна для заполнения;");
            }
            if (string.IsNullOrWhiteSpace(TBoxLogin.Text))
            {
                errorBuilder.AppendLine("Логин обязателен для заполнения;");
            }
            if (string.IsNullOrWhiteSpace(PBoxPassword.Password))
            {
                errorBuilder.AppendLine("Пароль обязателен для заполнения;");
            }
            User userFromDB = App.Context.Users.ToList().FirstOrDefault(p => p.Login.ToLower() == TBoxLogin.Text.ToLower());
            if (userFromDB != null)
            {
                errorBuilder.AppendLine("Пользователь с таким логином уже существует;");
            }
            if (errorBuilder.Length > 0)
            {
                errorBuilder.Insert(0, "Устраните следующие ошибки:\n");
            }
            return errorBuilder.ToString();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string errorMessage = CheckErrors();
                if (errorMessage.Length > 0)
                {
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                User user = new User();
                user.Login = TBoxLogin.Text;
                user.Password = PBoxPassword.Password;
                user.IdRole = 2;
                App.Context.Users.Add(user);
                App.Context.SaveChanges();
                Employee employee = new Employee();
                employee.LastName = TBoxLastName.Text;
                employee.FirstName = TBoxFirstName.Text;
                employee.MiddleName = TBoxMiddleName.Text;
                employee.Phone = TBoxPhone.Text;
                employee.Email = TBoxEmail.Text;
                employee.BirthDate = DPBirthDate.SelectedDate;
                employee.IdUser = user.IdUser;
                App.Context.Employees.Add(employee);
                App.Context.SaveChanges();
                MessageBox.Show("Сотрудник добавлен.");
                Clear();
            }
            catch
            {
                MessageBox.Show("Ошибка добавления сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Clear()
        {
            TBoxLastName.Clear();
            TBoxFirstName.Clear();
            TBoxMiddleName.Clear();
            TBoxPhone.Clear();
            TBoxEmail.Clear();
            DPBirthDate.SelectedDate = null;
            TBoxLogin.Clear();
            PBoxPassword.Clear();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow window = new AdminWindow();
            window.Show();
            this.Close();
        }
    }
}