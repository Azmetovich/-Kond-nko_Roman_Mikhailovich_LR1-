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
    public partial class AdminProductsWindow : System.Windows.Window
    {
        public AdminProductsWindow()
        {
            InitializeComponent();
            UpdateProducts();
        }
        private void UpdateProducts()
        {
            try
            {
                LViewProducts.ItemsSource = App.Context.Products.ToList();
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки товаров.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEditProductWindow window = new AddEditProductWindow();
            window.ShowDialog();
            UpdateProducts();
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Product currentProduct = (sender as Button).DataContext as Product;
            AddEditProductWindow window = new AddEditProductWindow(currentProduct);
            window.ShowDialog();
            UpdateProducts();
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Product currentProduct = (sender as Button).DataContext as Product;

                if (currentProduct == null)
                {
                    MessageBox.Show("Выберите товар.");
                    return;
                }

                if (MessageBox.Show("Вы уверены, что хотите удалить товар: " + currentProduct.ProductName + "?",
                    "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    App.Context.Products.Remove(currentProduct);
                    App.Context.SaveChanges();
                    UpdateProducts();
                    MessageBox.Show("Товар удален.");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка удаления товара. Возможно, товар уже используется в заказах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow window = new AdminWindow();
            window.Show();
            this.Close();
        }
    }
}