using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace Repo
{
    public class CartRepository
    {
        private static Cart _cart = new Cart();

        // Danh sách sản phẩm có sẵn
        private static List<Product> _availableProducts = new List<Product>
    {
        new Product { ProductId = "ML001", Name = "Thuoc diet chuot", Quantity = 10000, ExpiryDate = DateTime.Parse("2025-12-31"), Price = 10500.00 },
        new Product { ProductId = "ML002", Name = "Phan Bon UR", Quantity = 2000, ExpiryDate = DateTime.Parse("2025-06-30"), Price = 20000.00 },
        new Product { ProductId = "ML003", Name = "Phan Bon SR", Quantity = 1500, ExpiryDate = DateTime.Parse("2025-07-15"), Price = 30000.00 }
    };

        public List<Product> GetAvailableProducts()
        {
            return _availableProducts;
        }

        public List<Product> GetCartItems()
        {
            return _cart.Products;
        }

        public void AddProductToCart(Product product)
        {
            _cart.Products.Add(product);
        }
    }
}
