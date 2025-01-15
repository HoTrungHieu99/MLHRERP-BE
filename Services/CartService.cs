using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Repo;

namespace Services
{
    public class CartService
    {
        private readonly CartRepository _cartRepository;

        public CartService()
        {
            _cartRepository = new CartRepository();
        }

        public List<Product> GetCartItems()
        {
            return _cartRepository.GetCartItems();
        }

        public List<Product> GetAvailableProducts()
        {
            return _cartRepository.GetAvailableProducts();
        }

        public string AddProductToCart(string productId, int quantity)
        {
            var availableProducts = _cartRepository.GetAvailableProducts();
            var product = availableProducts.FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                return "Product not found.";
            }

            if (quantity > product.Quantity)
            {
                return "Insufficient stock.";
            }

            // Thêm sản phẩm vào giỏ hàng
            var productToAdd = new Product
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Quantity = quantity,
                ExpiryDate = product.ExpiryDate,
                Price = product.Price
            };

            // Cập nhật số lượng còn lại trong kho
            product.Quantity -= quantity;

            _cartRepository.AddProductToCart(productToAdd);
            return "Product added to cart successfully.";
        }

        public double GetCartTotalPrice()
        {
            var cartItems = _cartRepository.GetCartItems();
            return cartItems.Sum(p => p.Quantity * p.Price);
        }
    }
}
