using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;

namespace Services.Service
{
    public class TaxConfigService : ITaxConfigService
    {
        private readonly ITaxConfigRepository _taxConfigRepository;

        public TaxConfigService(ITaxConfigRepository taxConfigRepository)
        {
            _taxConfigRepository = taxConfigRepository;
        }

        public async Task<IEnumerable<TaxConfigDTO>> GetAllAsync()
        {
            var taxConfigs = await _taxConfigRepository.GetAllAsync();
            return taxConfigs.Select(tc => new TaxConfigDTO
            {
                TaxId = tc.TaxId,
                TaxName = tc.TaxName,
                TaxRate = tc.TaxRate,
                IsActive = tc.IsActive,
                Description = tc.Description
            });
        }

        public async Task<TaxConfigDTO> GetByIdAsync(int id)
        {
            var tc = await _taxConfigRepository.GetByIdAsync(id);
            if (tc == null) return null;

            return new TaxConfigDTO
            {
                TaxId = tc.TaxId,
                TaxName = tc.TaxName,
                TaxRate = tc.TaxRate,
                IsActive = tc.IsActive,
                Description = tc.Description
            };
        }

        public async Task<TaxConfigDTO> AddAsync(TaxConfigDTO taxConfigDto)
        {
            var taxConfig = new TaxConfig
            {
                TaxName = taxConfigDto.TaxName,
                TaxRate = taxConfigDto.TaxRate,
                IsActive = taxConfigDto.IsActive,
                Description = taxConfigDto.Description,
                CreatedAt = DateTime.Now
            };

            var result = await _taxConfigRepository.AddAsync(taxConfig);

            return new TaxConfigDTO
            {
                TaxId = result.TaxId,
                TaxName = result.TaxName,
                TaxRate = result.TaxRate,
                IsActive = result.IsActive,
                Description = result.Description
            };
        }

        public async Task<TaxConfigDTO> UpdateAsync(TaxConfigDTO taxConfigDto)
        {
            var taxConfig = await _taxConfigRepository.GetByIdAsync(taxConfigDto.TaxId);
            if (taxConfig == null) return null;

            taxConfig.TaxName = taxConfigDto.TaxName;
            taxConfig.TaxRate = taxConfigDto.TaxRate;
            taxConfig.IsActive = taxConfigDto.IsActive;
            taxConfig.Description = taxConfigDto.Description;
            taxConfig.UpdatedAt = DateTime.Now;

            var result = await _taxConfigRepository.UpdateAsync(taxConfig);

            return new TaxConfigDTO
            {
                TaxId = result.TaxId,
                TaxName = result.TaxName,
                TaxRate = result.TaxRate,
                IsActive = result.IsActive,
                Description = result.Description
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _taxConfigRepository.DeleteAsync(id);
        }
    }
}