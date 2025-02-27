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
    public class TaxConfigRepository : ITaxConfigRepository
    {
        private readonly MinhLongDbContext _context;

        public TaxConfigRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaxConfig>> GetAllAsync()
        {
            return await _context.TaxConfigs.ToListAsync();
        }

        public async Task<TaxConfig> GetByIdAsync(int id)
        {
            return await _context.TaxConfigs.FindAsync(id);
        }

        public async Task<TaxConfig> AddAsync(TaxConfig taxConfig)
        {
            _context.TaxConfigs.Add(taxConfig);
            await _context.SaveChangesAsync();
            return taxConfig;
        }

        public async Task<TaxConfig> UpdateAsync(TaxConfig taxConfig)
        {
            _context.TaxConfigs.Update(taxConfig);
            await _context.SaveChangesAsync();
            return taxConfig;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var taxConfig = await _context.TaxConfigs.FindAsync(id);
            if (taxConfig == null) return false;

            _context.TaxConfigs.Remove(taxConfig);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
