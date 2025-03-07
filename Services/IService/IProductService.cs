using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;
using BusinessObject.Models;

namespace Services.IService
{
    public interface IProductService
    {
        Task<PagedResult<Product>> GetProductsAsync(int page, int pageSize);
        Task<ProductResponseDto> GetProductByIdAsync(long id);
        Task<ProductResponseDto> CreateProductAsync(ProductDto productDto, Guid userId);
        Task<ProductResponseDto> UpdateProductAsync(long id, ProductDto productDto, Guid userId);
        Task<bool> DeleteProductAsync(long id);
        
    }
}
