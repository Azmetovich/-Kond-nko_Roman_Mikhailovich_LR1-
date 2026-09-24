using Microsoft.Win32;
using Pizza.Entity;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
namespace Pizza
{
    public partial class AddEditProductWindow : System.Windows.Window
    {
        private Product currentProduct = null;
        private string imagePath = "";
        private List<IngredientInfo> allIngredients = new List<IngredientInfo>();
        private List<IngredientInfo> productIngredientsList = new List<IngredientInfo>();
        public AddEditProductWindow()
        {
            InitializeComponent();
            TbTitle.Text = "Добавление товара";
            LoadIngredients();
        }
        public AddEditProductWindow(Product product)
        {
            InitializeComponent();
            currentProduct = product;
            TbTitle.Text = "Редактирование товара";
            TBoxProductName.Text = currentProduct.ProductName;
            TBoxDescription.Text = currentProduct.Description;
            TBoxPrice.Text = currentProduct.Price.ToString("N2");
            imagePath = currentProduct.ImagePath;
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                ImageProduct.Source = new BitmapImage(new System.Uri(imagePath));
            }
            LoadIngredients();
        }
        private string CheckErrors()
        {
            StringBuilder errorBuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(TBoxProductName.Text))
            {
                errorBuilder.AppendLine("Название товара обязательно для заполнения;");
            }
            Product productFromDB = App.Context.Products.ToList().FirstOrDefault(p =>
                p.ProductName.ToLower() == TBoxProductName.Text.ToLower());
            if (productFromDB != null && productFromDB != currentProduct)
            {
                errorBuilder.AppendLine("Такой товар уже есть в базе данных;");
            }
            decimal price = 0;
            if (decimal.TryParse(TBoxPrice.Text, out price) == false || price <= 0)
            {
                errorBuilder.AppendLine("Цена товара должна быть положительным числом;");
            }
            if (string.IsNullOrWhiteSpace(TBoxDescription.Text))
            {
                errorBuilder.AppendLine("Описание товара обязательно для заполнения;");
            }
            if (errorBuilder.Length > 0)
            {
                errorBuilder.Insert(0, "Устраните следующие ошибки:\n");
            }
            return errorBuilder.ToString();
        }
        private void LoadIngredients()
        {
            try
            {
                allIngredients = new List<IngredientInfo>();
                productIngredientsList = new List<IngredientInfo>();
                var ingredients = App.Context.Ingredients.OrderBy(p => p.IngredientName).ToList();
                foreach (var ingredient in ingredients)
                {
                    IngredientInfo info = new IngredientInfo();
                    info.IdIngredient = ingredient.IdIngredient;
                    info.IngredientName = ingredient.IngredientName;
                    allIngredients.Add(info);
                }
                if (currentProduct != null)
                {
                    var productIngredients = App.Context.ProductIngredients
                        .Where(p => p.IdProduct == currentProduct.IdProduct)
                        .ToList();
                    foreach (var productIngredient in productIngredients)
                    {
                        Ingredient ingredient = App.Context.Ingredients.FirstOrDefault(p => p.IdIngredient == productIngredient.IdIngredient);
                        if (ingredient != null)
                        {
                            IngredientInfo info = new IngredientInfo();
                            info.IdIngredient = ingredient.IdIngredient;
                            info.IngredientName = ingredient.IngredientName;
                            productIngredientsList.Add(info);
                        }
                    }
                }
                RefreshIngredients();
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки ингредиентов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RefreshIngredients()
        {
            CBoxIngredients.ItemsSource = null;
            CBoxIngredients.ItemsSource = allIngredients;
            LBoxProductIngredients.ItemsSource = null;
            LBoxProductIngredients.ItemsSource = productIngredientsList;
        }
        private void BtnAddIngredientToProduct_Click(object sender, RoutedEventArgs e)
        {
            IngredientInfo selectedIngredient = CBoxIngredients.SelectedItem as IngredientInfo;
            if (selectedIngredient == null)
            {
                MessageBox.Show("Выберите ингредиент.");
                return;
            }
            bool alreadyExists = productIngredientsList.Any(p => p.IdIngredient == selectedIngredient.IdIngredient);
            if (alreadyExists)
            {
                MessageBox.Show("Этот ингредиент уже есть в составе.");
                return;
            }
            productIngredientsList.Add(selectedIngredient);
            RefreshIngredients();
        }
        private void BtnRemoveIngredientFromProduct_Click(object sender, RoutedEventArgs e)
        {
            IngredientInfo selectedIngredient = LBoxProductIngredients.SelectedItem as IngredientInfo;
            if (selectedIngredient == null)
            {
                MessageBox.Show("Выберите ингредиент для удаления.");
                return;
            }
            productIngredientsList.Remove(selectedIngredient);
            RefreshIngredients();
        }
        private void BtnCreateIngredient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ingredientName = TBoxNewIngredient.Text.Trim();
                if (string.IsNullOrWhiteSpace(ingredientName))
                {
                    MessageBox.Show("Введите название ингредиента.");
                    return;
                }
                Ingredient ingredientFromDB = App.Context.Ingredients.ToList().FirstOrDefault(p =>
                    p.IngredientName.ToLower() == ingredientName.ToLower());
                if (ingredientFromDB != null)
                {
                    MessageBox.Show("Такой ингредиент уже есть.");
                    return;
                }
                Ingredient newIngredient = new Ingredient();
                newIngredient.IngredientName = ingredientName;
                App.Context.Ingredients.Add(newIngredient);
                App.Context.SaveChanges();
                IngredientInfo info = new IngredientInfo();
                info.IdIngredient = newIngredient.IdIngredient;
                info.IngredientName = newIngredient.IngredientName;
                allIngredients.Add(info);
                productIngredientsList.Add(info);
                TBoxNewIngredient.Clear();
                RefreshIngredients();
                MessageBox.Show("Ингредиент добавлен.");
            }
            catch
            {
                MessageBox.Show("Ошибка добавления ингредиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveProductIngredients(Product product)
        {
            var oldIngredients = App.Context.ProductIngredients
                .Where(p => p.IdProduct == product.IdProduct)
                .ToList();
            foreach (var oldIngredient in oldIngredients)
            {
                App.Context.ProductIngredients.Remove(oldIngredient);
            }
            foreach (var ingredient in productIngredientsList)
            {
                ProductIngredient productIngredient = new ProductIngredient();
                productIngredient.IdProduct = product.IdProduct;
                productIngredient.IdIngredient = ingredient.IdIngredient;
                App.Context.ProductIngredients.Add(productIngredient);
            }
            App.Context.SaveChanges();
        }
        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Картинки|*.png;*.jpg;*.jpeg";
                if (dialog.ShowDialog() == true)
                {
                    imagePath = dialog.FileName;
                    ImageProduct.Source = new BitmapImage(new System.Uri(imagePath));
                }
            }
            catch
            {
                MessageBox.Show("Ошибка выбора изображения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string errorMessage = CheckErrors();
                if (errorMessage.Length > 0)
                {
                    MessageBox.Show(errorMessage, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (currentProduct == null)
                {
                    Product product = new Product();
                    product.ProductName = TBoxProductName.Text;
                    product.Description = TBoxDescription.Text;
                    product.Price = decimal.Parse(TBoxPrice.Text);
                    product.ImagePath = imagePath;
                    App.Context.Products.Add(product);
                    App.Context.SaveChanges();
                    SaveProductIngredients(product);
                    MessageBox.Show("Товар добавлен.");
                }
                else
                {
                    currentProduct.ProductName = TBoxProductName.Text;
                    currentProduct.Description = TBoxDescription.Text;
                    currentProduct.Price = decimal.Parse(TBoxPrice.Text);
                    currentProduct.ImagePath = imagePath;
                    App.Context.SaveChanges();
                    SaveProductIngredients(currentProduct);
                    MessageBox.Show("Товар изменен.");
                }
                this.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    public class IngredientInfo
    {
        public int IdIngredient { get; set; }
        public string IngredientName { get; set; }
    }
}