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
using System.Windows.Shapes;

namespace Pizza
{
    public partial class OrderHistoryWindow : System.Windows.Window
    {
        public OrderHistoryWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                List<OrderHistoryInfo> orderList = new List<OrderHistoryInfo>();

                var orders = App.Context.Orders
                    .OrderByDescending(p => p.OrderDate)
                    .ToList();

                foreach (var order in orders)
                {
                    Employee employee = App.Context.Employees.FirstOrDefault(p => p.IdEmployee == order.IdEmployee);

                    var orderProducts = App.Context.OrderProducts
                        .Where(p => p.IdOrder == order.IdOrder)
                        .ToList();

                    List<string> productNames = new List<string>();

                    foreach (var orderProduct in orderProducts)
                    {
                        Product product = App.Context.Products.FirstOrDefault(p => p.IdProduct == orderProduct.IdProduct);

                        if (product != null)
                        {
                            productNames.Add(product.ProductName + " - " + orderProduct.Price.ToString("0.00") + " руб.");
                        }
                    }

                    OrderHistoryInfo info = new OrderHistoryInfo();

                    info.IdOrder = order.IdOrder;
                    info.ReceiptNumberText = "Чек № " + order.ReceiptNumber;
                    info.OrderDateText = "Дата заказа: " + order.OrderDate.ToString("dd.MM.yyyy HH:mm");

                    if (employee != null)
                    {
                        info.EmployeeName = "Сотрудник: " + employee.LastName + " " + employee.FirstName;
                    }
                    else
                    {
                        info.EmployeeName = "Сотрудник: не найден";
                    }

                    info.ProductsText = "Товары: " + string.Join(", ", productNames);
                    info.TotalPriceText = "Итого: " + order.TotalPrice.ToString("0.00") + " руб.";

                    orderList.Add(info);
                }

                LViewOrders.ItemsSource = orderList;
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки истории заказов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow window = new AdminWindow();
            window.Show();
            this.Close();
        }
    }

    public class OrderHistoryInfo
    {
        public int IdOrder { get; set; }
        public string ReceiptNumberText { get; set; }
        public string OrderDateText { get; set; }
        public string EmployeeName { get; set; }
        public string ProductsText { get; set; }
        public string TotalPriceText { get; set; }
    }
}
