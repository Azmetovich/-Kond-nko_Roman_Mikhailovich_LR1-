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
    public partial class WorkSchedule : System.Windows.Window
    {
        public WorkSchedule()
        {
            InitializeComponent();
            LoadSchedule();
        }

        private Employee GetCurrentEmployee()
        {
            Employee employee = App.Context.Employees.FirstOrDefault(p => p.IdUser == App.CurrentUser.IdUser);
            return employee;
        }

        private void LoadSchedule()
        {
            try
            {
                List<ScheduleInfo> scheduleList = new List<ScheduleInfo>();

                var schedules = App.Context.WorkSchedules
                    .OrderBy(p => p.WorkDate)
                    .ToList();

                foreach (var schedule in schedules)
                {
                    Employee employee = App.Context.Employees.FirstOrDefault(p => p.IdEmployee == schedule.IdEmployee);

                    if (employee != null)
                    {
                        ScheduleInfo info = new ScheduleInfo();
                        info.IdSchedule = schedule.IdSchedule;
                        info.WorkDate = schedule.WorkDate;
                        info.WorkDateText = "Рабочий день: " + schedule.WorkDate.ToString("dd.MM.yyyy");
                        info.EmployeeName = employee.LastName + " " + employee.FirstName;

                        scheduleList.Add(info);
                    }
                }

                LViewSchedule.ItemsSource = scheduleList;
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки графика работы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Employee employee = GetCurrentEmployee();

                if (employee == null)
                {
                    MessageBox.Show("Данные сотрудника не найдены.");
                    return;
                }

                if (DPWorkDate.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату.");
                    return;
                }

                DateTime selectedDate = Convert.ToDateTime(DPWorkDate.SelectedDate);

                Pizza.Entity.WorkSchedule schedule = new Pizza.Entity.WorkSchedule();
                schedule.IdEmployee = employee.IdEmployee;
                schedule.WorkDate = selectedDate;

                App.Context.WorkSchedules.Add(schedule);
                App.Context.SaveChanges();

                MessageBox.Show("Рабочий день добавлен.");
                DPWorkDate.SelectedDate = null;
                LoadSchedule();
            }
            catch
            {
                MessageBox.Show("Ошибка добавления рабочего дня.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser.IdRole == 1)
            {
                AdminWindow window = new AdminWindow();
                window.Show();
                this.Close();
            }
            else
            {
                WorkerWindow window = new WorkerWindow();
                window.Show();
                this.Close();
            }
        }
    }

    public class ScheduleInfo
    {
        public int IdSchedule { get; set; }
        public DateTime WorkDate { get; set; }
        public string WorkDateText { get; set; }
        public string EmployeeName { get; set; }
    }
}