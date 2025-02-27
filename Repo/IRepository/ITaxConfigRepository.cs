using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repo.IRepository
{
    public interface ITaxConfigRepository
    {
        Task<IEnumerable<TaxConfig>> GetAllAsync();
        Task<TaxConfig> GetByIdAsync(int id);
        Task<TaxConfig> AddAsync(TaxConfig taxConfig);
        Task<TaxConfig> UpdateAsync(TaxConfig taxConfig);
        Task<bool> DeleteAsync(int id);
    }
}
