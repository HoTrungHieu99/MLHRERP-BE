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
        private readonly IImageService _imageService;

        public ProductService(IProductRepository repository, IImageService imageService)
        {
            _repository = repository;
            _imageService = imageService;
        }

        public async Task<List<ProductResponseDto>> GetProductsAsync()
        {
            var products = await _repository.GetProductsAsync();
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
                AvailableStock = p.AvailableStock,
                Price = p.Price,
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
                AvailableStock = product.AvailableStock,
                Price = product.Price,
                // ✅ Lấy danh sách URL hình ảnh từ database
                Images = product.Images.Select(img => img.ImageUrl).ToList()
            };
        }

        /*public async Task<ProductResponseDto> CreateProductAsync(ProductDto productDto, Guid userId)
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
                CreatedDate = DateTime.Now
            };

            // ✅ Lưu sản phẩm và danh sách hình ảnh
            var createdProduct = await _repository.AddAsync(product, productDto.Images);

            return await GetProductByIdAsync(createdProduct.ProductId);
        }*/

        public async Task<ProductResponseDto> CreateProductAsync(ProductDto model, Guid userId)
        {
            var product = new Product
            {
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                Unit = model.Unit,
                DefaultExpiration = model.DefaultExpiration,
                CategoryId = model.CategoryId,
                Description = model.Description,
                TaxId = model.TaxId,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            };

            // ✅ Tạo product trước
            var createdProduct = await _repository.AddAsync(product); // KHÔNG truyền imageUrls

            // ✅ Nếu có ảnh, upload ảnh và lưu vào bảng Image
            if (model.Images != null && model.Images.Count > 0)
            {
                var imageModel = new ImageModel
                {
                    Files = model.Images
                };

                await _imageService.UploadImagesAsync(imageModel, createdProduct.ProductId);
            }

            return await GetProductByIdAsync(createdProduct.ProductId);
        }




        public async Task<ProductResponseDto> UpdateProductAsync(long id, UpdateProductDTO productDto, Guid userId)
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
            product.UpdatedDate = DateTime.Now;

            var updatedProduct = await _repository.UpdateAsync(product);
            return await GetProductByIdAsync(updatedProduct.ProductId);
        }

        public async Task<bool> DeleteProductAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }


        public async Task<List<ProductSimpleResponseDto>> GetProductsByCategoryIdAsync(long categoryId)
        {
            var products = await _repository.GetProductsByCategoryIdAsync(categoryId);
            return products.Select(p => new ProductSimpleResponseDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Images = p.Images.Select(img => img.ImageUrl).ToList()
            }).ToList();
        }

    }
}
