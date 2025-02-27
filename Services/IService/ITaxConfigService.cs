using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;

namespace Services.IService
{
    public interface ITaxConfigService
    {
        Task<IEnumerable<TaxConfigDTO>> GetAllAsync();
        Task<TaxConfigDTO> GetByIdAsync(int id);
        Task<TaxConfigDTO> AddAsync(TaxConfigDTO taxConfigDto);
        Task<TaxConfigDTO> UpdateAsync(TaxConfigDTO taxConfigDto);
        Task<bool> DeleteAsync(int id);
    }
}
