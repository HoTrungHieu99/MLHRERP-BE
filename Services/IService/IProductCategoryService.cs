using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.Product;
using BusinessObject.Models;

namespace Services.IService
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategoryResponseDto>> GetAllCategoriesAsync();
        Task<ProductCategoryResponseDto> GetCategoryByIdAsync(long id);
        Task<ProductCategoryResponseDto> CreateCategoryAsync(ProductCategoryDto categoryDto, Guid userId);
        Task<ProductCategoryResponseDto> UpdateCategoryAsync(long id, ProductCategoryDto categoryDto, Guid userId);
        Task<bool> DeleteCategoryAsync(long id);
    }
}
