using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessLayer;
using Repo.IRepository;

namespace Repo.Repository
{
    public class AgencyAccountLevelRepository : IAgencyAccountLevelRepository
    {
        private readonly MinhLongDbContext _context;

        public AgencyAccountLevelRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AgencyAccountLevel entity)
        {
            await _context.AgencyAccountLevels.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
