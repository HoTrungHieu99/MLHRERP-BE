using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repo.IRepository
{
    public interface IProductCategoryRepository
    {
        //Task<IEnumerable<ProductCategory>> GetAllAsync();
        Task<List<ProductCategory>> GetAllAsync();
        Task<ProductCategory> GetByIdAsync(long id);
        Task<ProductCategory> AddAsync(ProductCategory category);
        Task<ProductCategory> UpdateAsync(ProductCategory category);
        Task<bool> DeleteAsync(long id);
    }
}
