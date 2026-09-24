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
    public partial class CatalogWindow : System.Windows.Window
    {
        public CatalogWindow()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                List<CatalogProduct> productsList = new List<CatalogProduct>();

                var products = App.Context.Products.ToList();

                foreach (var product in products)
                {
                    var ingredients = App.Context.ProductIngredients
                        .Where(p => p.IdProduct == product.IdProduct)
                        .Select(p => p.Ingredient.IngredientName)
                        .ToList();

                    CatalogProduct catalogProduct = new CatalogProduct();

                    catalogProduct.IdProduct = product.IdProduct;
                    catalogProduct.ProductName = product.ProductName;
                    catalogProduct.Description = product.Description;
                    catalogProduct.Price = product.Price;
                    catalogProduct.PriceText = product.Price.ToString("0.00") + " руб.";
                    catalogProduct.Ingredients = "состав: " + string.Join(", ", ingredients);
                    catalogProduct.ImagePath = product.ImagePath;

                    productsList.Add(catalogProduct);
                }

                ProductsList.ItemsSource = productsList;
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки каталога товаров.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CatalogProduct selectedProduct = ProductsList.SelectedItem as CatalogProduct;

                if (selectedProduct == null)
                {
                    MessageBox.Show("Выберите товар.");
                    return;
                }

                Cart cart = new Cart();
                cart.IdUser = App.CurrentUser.IdUser;
                cart.IdProduct = selectedProduct.IdProduct;
                cart.DateAdd = DateTime.Now;

                App.Context.Carts.Add(cart);
                App.Context.SaveChanges();

                MessageBox.Show("Товар добавлен в корзину.");
            }
            catch
            {
                MessageBox.Show("Ошибка добавления товара в корзину.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            WorkerWindow workerWindow = new WorkerWindow();
            workerWindow.Show();
            this.Close();
        }
    }
}