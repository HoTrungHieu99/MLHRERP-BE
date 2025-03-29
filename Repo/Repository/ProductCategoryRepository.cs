using System;
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
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly MinhLongDbContext _context;

        public ProductCategoryRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        /*public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories.ToListAsync();
        }*/

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories
                .Include(c => c.Creator)
                    .ThenInclude(u => u.Employee)
                .Include(c => c.Updater)
                    .ThenInclude(u => u.Employee)
                .ToListAsync();
        }


        /*public async Task<ProductCategory> GetByIdAsync(long id)
        {
            return await _context.ProductCategories.FindAsync(id);
        }*/

        public async Task<ProductCategory> GetByIdAsync(long id)
        {
            return await _context.ProductCategories
                .Include(c => c.Creator)
                    .ThenInclude(u => u.Employee)
                .Include(c => c.Updater)
                    .ThenInclude(u => u.Employee)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }


        public async Task<ProductCategory> AddAsync(ProductCategory category)
        {
            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<ProductCategory> UpdateAsync(ProductCategory category)
        {
            _context.ProductCategories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return false;
            }
            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
