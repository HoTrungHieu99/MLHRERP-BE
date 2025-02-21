using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    public class AgencyAccountRepository : IAgencyAccountRepository
    {
        private readonly MinhLongDbContext _context;

        public AgencyAccountRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<AgencyAccount> GetAgencyAccountByIdAsync(long agencyId)
        {
            return _context.AgencyAccounts.FirstOrDefault(e => e.AgencyId == agencyId);
        }

        public async Task<List<AgencyAccount>> GetAll()
        {
            return await _context.AgencyAccounts.ToListAsync();
        }

        public async Task<bool> UpdateAgencyAccountAsync(AgencyAccount agencyAccount)
        {
            var existingAgencyAccount = await _context.AgencyAccounts.FindAsync(agencyAccount.AgencyId);
            if (existingAgencyAccount == null)
                return false;

            // Cập nhật thông tin
            existingAgencyAccount.AgencyName = agencyAccount.AgencyName;
            existingAgencyAccount.Address = agencyAccount.Address;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
