using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.AgencyLevel;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;

namespace Services.Service
{
    public class AgencyLevelService : IAgencyLevelService
    {
        private readonly IAgencyLevelRepository _repo;

        public AgencyLevelService(IAgencyLevelRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AgencyLevel>> GetAllLevelsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<AgencyLevel?> GetLevelByIdAsync(long id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task CreateLevelAsync(CreateAgencyLevelDto dto)
        {
            var level = new AgencyLevel
            {
                LevelName = dto.LevelName,
                DiscountPercentage = dto.DiscountPercentage,
                CreditLimit = dto.CreditLimit,
                PaymentTerm = dto.PaymentTerm
            };
            await _repo.AddAsync(level);
        }

        public async Task UpdateLevelAsync(long id, UpdateAgencyLevelDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Agency level not found");

            existing.LevelName = dto.LevelName;
            existing.DiscountPercentage = dto.DiscountPercentage;
            existing.CreditLimit = dto.CreditLimit;
            existing.PaymentTerm = dto.PaymentTerm;

            await _repo.UpdateAsync(existing);
        }


        public async Task DeleteLevelAsync(long id)
        {
            await _repo.DeleteAsync(id);
        }
    }


}
