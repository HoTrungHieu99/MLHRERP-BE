using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;
using BusinessObject.Models;
using Repo.IRepository;
using Repo.Repository;
using Services.IService;

namespace Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductResponseDto>> GetProductsAsync(int page, int pageSize)
        {
            var products = await _repository.GetProductsAsync((page - 1) * pageSize, pageSize);
            return products.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Unit = p.Unit,
                DefaultExpiration = p.DefaultExpiration,
                CategoryId = p.CategoryId,
                Description = p.Description,
                TaxId = p.TaxId,
                CreatedBy = p.CreatedBy,
                CreatedDate = p.CreatedDate,
                UpdatedBy = p.UpdatedBy,
                UpdatedDate = p.UpdatedDate,
                Images = p.Images.Select(img => img.ImageUrl).ToList()
            }).ToList();
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(long id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Unit = product.Unit,
                DefaultExpiration = product.DefaultExpiration,
                CategoryId = product.CategoryId,
                Description = product.Description,
                TaxId = product.TaxId,
                CreatedBy = product.CreatedBy,
                CreatedDate = product.CreatedDate,
                UpdatedBy = product.UpdatedBy,
                UpdatedDate = product.UpdatedDate,

                // ✅ Lấy danh sách URL hình ảnh từ database
                Images = product.Images.Select(img => img.ImageUrl).ToList()
            };
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductDto productDto, Guid userId)
        {
            var product = new Product
            {
                ProductCode = productDto.ProductCode,
                ProductName = productDto.ProductName,
                Unit = productDto.Unit,
                DefaultExpiration = productDto.DefaultExpiration,
                CategoryId = productDto.CategoryId,
                Description = productDto.Description,
                TaxId = productDto.TaxId,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };

            // ✅ Lưu sản phẩm và danh sách hình ảnh
            var createdProduct = await _repository.AddAsync(product, productDto.Images);

            return await GetProductByIdAsync(createdProduct.ProductId);
        }


        public async Task<ProductResponseDto> UpdateProductAsync(long id, ProductDto productDto, Guid userId)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;

            product.ProductName = productDto.ProductName;
            product.Unit = productDto.Unit;
            product.DefaultExpiration = productDto.DefaultExpiration;
            product.CategoryId = productDto.CategoryId;
            product.Description = productDto.Description;
            product.TaxId = productDto.TaxId;
            product.UpdatedBy = userId;
            product.UpdatedDate = DateTime.UtcNow;

            var updatedProduct = await _repository.UpdateAsync(product, productDto.Images);
            return await GetProductByIdAsync(updatedProduct.ProductId);
        }

        public async Task<bool> DeleteProductAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }

        
    }
}
