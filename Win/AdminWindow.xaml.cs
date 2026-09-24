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
    public partial class AdminWindow : System.Windows.Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }
        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            AdminProductsWindow window = new AdminProductsWindow();
            window.Show();
            this.Close();
        }
        private void BtnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeWindow window = new AddEmployeeWindow();
            window.Show();
            this.Close();
        }
        private void BtnOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            OrderHistoryWindow order = new OrderHistoryWindow();
            order.Show();
            this.Close();
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
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