using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repo.IRepository
{
    public interface IProductRepository
    {
        Task<int> GetTotalProductsAsync(); // ✅ Thêm phương thức này
        Task<List<Product>> GetProductsAsync(int skip, int take);
        Task<Product> GetByIdAsync(long id);
        Task<Product> AddAsync(Product product, List<string> imageUrls);
        Task<Product> UpdateAsync(Product product, List<string> imageUrls);
        Task<bool> DeleteAsync(long id);
        
    }
}
