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
    public class AgencyAccountRepository : IAgencyAccountRepository
    {
        private readonly MinhLongDbContext _context;

        public AgencyAccountRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<AgencyAccount> CreateAgencyAccountAsync(AgencyAccount account)
        {
            _context.AgencyAccounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<AgencyAccount> GetByUserIdAsync(Guid userId)
        {
            return await _context.AgencyAccounts.FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task<AgencyAccount?> GetByUsernameAsync(string username)
        {
            return await _context.AgencyAccounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.User.Username == username);
        }

        public async Task<AgencyAccount?> GetByUserIdWithLevelsAsync(Guid userId)
        {
            return await _context.AgencyAccounts
                .Include(a => a.AgencyAccountLevels)
                    .ThenInclude(al => al.Level)
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }

    }

}
