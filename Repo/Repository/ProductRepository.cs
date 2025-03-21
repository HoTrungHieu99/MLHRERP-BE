﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;

namespace Repo.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly MinhLongDbContext _context;

        public ProductRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalProductsAsync() // ✅ Triển khai phương thức này
        {
            return await _context.Products.CountAsync();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Images) // ✅ Bao gồm hình ảnh
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(long id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.TaxConfig)
                .Include(p => p.Images) // ✅ Bao gồm hình ảnh
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        /*public async Task<Product> AddAsync(Product product, List<string> imageUrls)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync(); // 🔥 Lưu sản phẩm trước để lấy ProductId

            // ✅ Lưu hình ảnh vào bảng Image
            if (imageUrls != null && imageUrls.Count > 0)
            {
                foreach (var imageUrl in imageUrls)
                {
                    var image = new Image
                    {
                        ProductId = product.ProductId,
                        ImageUrl = imageUrl
                    };
                    _context.Images.Add(image);
                }
                await _context.SaveChangesAsync(); // 🔥 Lưu hình ảnh
            }

            return product;
        }*/

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync(); // 🔥 Lưu để có ProductId

            return product;
        }


        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            // ✅ Xóa hình ảnh cũ trước khi cập nhật
            var existingImages = _context.Images.Where(img => img.ProductId == product.ProductId);
            _context.Images.RemoveRange(existingImages);
            await _context.SaveChangesAsync();

            /*// ✅ Thêm hình ảnh mới
            foreach (var imageUrl in imageUrls)
            {
                _context.Images.Add(new Image
                {
                    ProductId = product.ProductId,
                    ImageUrl = imageUrl
                });
            }*/
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Product>> GetProductsByCategoryIdAsync(long categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Images) // Load danh sách ảnh
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(long productId) // ✅ Đổi tên cho dễ hiểu
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.TaxConfig)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

    }
}
