using Pizza.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pizza
{
    public partial class App : Application
    {
        public static PizzaParovozovEntities Context = new PizzaParovozovEntities();
        public static User CurrentUser;
    }

    public class CatalogProduct
    {
        public int IdCart { get; set; }
        public int IdProduct { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PriceText { get; set; }
        public string Ingredients { get; set; }
        public string ImagePath { get; set; }
    }
}