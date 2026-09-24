using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using Pizza.Entity;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
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
    public partial class CartWindow : System.Windows.Window
    {
        private List<CatalogProduct> cartProducts = new List<CatalogProduct>();
        public CartWindow()
        {
            InitializeComponent();
            LoadCart();
        }
        private Employee GetCurrentEmployee()
        {
            Employee employee = App.Context.Employees.FirstOrDefault(p => p.IdUser == App.CurrentUser.IdUser);
            return employee;
        }
        private void LoadCart()
        {
            cartProducts = new List<CatalogProduct>();
            var cartItems = App.Context.Carts.Where(p => p.IdUser == App.CurrentUser.IdUser).ToList();
            foreach (var cartItem in cartItems)
            {
                Product product = App.Context.Products.FirstOrDefault(p => p.IdProduct == cartItem.IdProduct);
                if (product != null)
                {
                    var ingredients = App.Context.ProductIngredients.Where(p => p.IdProduct == product.IdProduct).Select(p => p.Ingredient.IngredientName).ToList();
                    CatalogProduct catalogProduct = new CatalogProduct();
                    catalogProduct.IdCart = cartItem.IdCart;
                    catalogProduct.IdProduct = product.IdProduct;
                    catalogProduct.ProductName = product.ProductName;
                    catalogProduct.Description = product.Description;
                    catalogProduct.Price = product.Price;
                    catalogProduct.PriceText = product.Price.ToString("0.00") + " руб.";
                    catalogProduct.Ingredients = "состав: " + string.Join(", ", ingredients);
                    cartProducts.Add(catalogProduct);
                }
            }
            CartList.ItemsSource = null;
            CartList.ItemsSource = cartProducts;
            decimal totalPrice = cartProducts.Sum(p => p.Price);
            tbTotalPrice.Text = "Итого: " + totalPrice.ToString("0.00") + " руб.";
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            CatalogProduct selectedProduct = CartList.SelectedItem as CatalogProduct;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для удаления.");
                return;
            }
            Cart cart = App.Context.Carts.FirstOrDefault(p => p.IdCart == selectedProduct.IdCart);
            if (cart != null)
            {
                App.Context.Carts.Remove(cart);
                App.Context.SaveChanges();
                LoadCart();
                MessageBox.Show("Товар удален из корзины.");
            }
        }
        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cartProducts.Count == 0)
                {
                    MessageBox.Show("Корзина пустая.");
                    return;
                }
                Employee employee = GetCurrentEmployee();
                if (employee == null)
                {
                    MessageBox.Show("Данные сотрудника не найдены.");
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF файл|*.pdf";
                saveFileDialog.FileName = "Чек_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                if (saveFileDialog.ShowDialog() != true)
                {
                    return;
                }
                decimal totalPrice = cartProducts.Sum(p => p.Price);
                string receiptNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
                string filePath = saveFileDialog.FileName;
                Order order = new Order();
                order.IdEmployee = employee.IdEmployee;
                order.OrderDate = DateTime.Now;
                order.TotalPrice = totalPrice;
                order.ReceiptNumber = receiptNumber;
                string qrText = "Чек №" + receiptNumber + "\n" +
                "Дата: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n" +
                "Сумма: " + totalPrice.ToString("0.00") + " руб.";
                order.QRCodeText = qrText;
                order.ReceiptFilePath = filePath;
                App.Context.Orders.Add(order);
                App.Context.SaveChanges();
                foreach (CatalogProduct product in cartProducts)
                {
                    OrderProduct orderProduct = new OrderProduct();
                    orderProduct.IdOrder = order.IdOrder;
                    orderProduct.IdProduct = product.IdProduct;
                    orderProduct.Quantity = 1;
                    orderProduct.Price = product.Price;
                    App.Context.OrderProducts.Add(orderProduct);
                }
                App.Context.SaveChanges();
                GenerateReceiptPdf(filePath, receiptNumber, employee, cartProducts, totalPrice, qrText);
                var cartItems = App.Context.Carts.Where(p => p.IdUser == App.CurrentUser.IdUser).ToList();
                foreach (Cart cart in cartItems)
                {
                    App.Context.Carts.Remove(cart);
                }
                App.Context.SaveChanges();
                MessageBox.Show("Покупка оформлена. Чек сохранен.");
                LoadCart();
            }
            catch
            {
                MessageBox.Show("Ошибка оформления покупки или создания чека.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GenerateReceiptPdf(string filePath, string receiptNumber, Employee employee, List<CatalogProduct> products, decimal totalPrice, string qrText)
        {
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new MyFontResolver();
            }

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Чек покупки";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont fontHeader = new XFont("Arial", 18, XFontStyleEx.Bold);
            XFont fontRegular = new XFont("Arial", 12, XFontStyleEx.Regular);
            XFont fontBold = new XFont("Arial", 12, XFontStyleEx.Bold);

            double y = 40;

            gfx.DrawString("ПиццаФабрика", fontHeader, XBrushes.Black,
                new XRect(0, y, page.Width.Point, 30), XStringFormats.TopCenter);

            y += 45;
            gfx.DrawString("Чек № " + receiptNumber, fontBold, XBrushes.Black, 40, y);

            y += 25;
            gfx.DrawString("Дата: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"), fontRegular, XBrushes.Black, 40, y);

            y += 20;
            gfx.DrawString("Сотрудник: " + employee.LastName + " " + employee.FirstName, fontRegular, XBrushes.Black, 40, y);

            y += 35;
            gfx.DrawString("Список товаров:", fontBold, XBrushes.Black, 40, y);

            y += 25;
            gfx.DrawLine(XPens.Black, 40, y, page.Width.Point - 40, y);

            y += 20;

            foreach (CatalogProduct product in products)
            {
                gfx.DrawString(product.ProductName, fontRegular, XBrushes.Black, 40, y);

                gfx.DrawString(product.Price.ToString("0.00") + " руб.", fontRegular, XBrushes.Black,
                    new XRect(350, y - 12, 150, 20), XStringFormats.TopRight);

                y += 22;
            }

            y += 10;
            gfx.DrawLine(XPens.Black, 40, y, page.Width.Point - 40, y);

            y += 30;
            gfx.DrawString("Итого: " + totalPrice.ToString("0.00") + " руб.", fontHeader, XBrushes.Black,
                new XRect(40, y, page.Width.Point - 80, 30), XStringFormats.TopRight);

            y += 50;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            System.Drawing.Bitmap qrBitmap = qrCode.GetGraphic(20);

            MemoryStream stream = new MemoryStream();
            qrBitmap.Save(stream, ImageFormat.Png);
            stream.Position = 0;

            XImage qrImage = XImage.FromStream(stream);

            double qrSize = 150;
            double qrX = (page.Width.Point - qrSize) / 2;

            gfx.DrawImage(qrImage, qrX, y, qrSize, qrSize);

            document.Save(filePath);
        }

        //Visual Studio для PDF не находит шрифт поэтому находим его вручную
        public class MyFontResolver : IFontResolver
        {
            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                if (isBold)
                {
                    return new FontResolverInfo("ArialBold");
                }
                return new FontResolverInfo("ArialRegular");
            }
            public byte[] GetFont(string faceName)
            {
                if (faceName == "ArialBold")
                {
                    return File.ReadAllBytes(@"C:\Windows\Fonts\arialbd.ttf");
                }
                return File.ReadAllBytes(@"C:\Windows\Fonts\arial.ttf");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            WorkerWindow worker = new WorkerWindow();
            worker.Show();
            this.Close();
        }
    }
}