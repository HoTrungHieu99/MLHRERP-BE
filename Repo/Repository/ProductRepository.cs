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

        public async Task<List<Product>> GetProductsAsync(int skip, int take)
        {
            return await _context.Products.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(long id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.TaxConfig)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
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

        
    }
}
