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
    public class AgencyLevelRepository : IAgencyLevelRepository
    {
        private readonly MinhLongDbContext _context;

        public AgencyLevelRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AgencyLevel>> GetAllAsync()
        {
            return await _context.AgencyLevels.ToListAsync();
        }

        public async Task<AgencyLevel?> GetByIdAsync(long id)
        {
            return await _context.AgencyLevels.FindAsync(id);
        }

        public async Task AddAsync(AgencyLevel level)
        {
            await _context.AgencyLevels.AddAsync(level);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AgencyLevel level)
        {
            _context.AgencyLevels.Update(level);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var level = await _context.AgencyLevels.FindAsync(id);
            if (level != null)
            {
                _context.AgencyLevels.Remove(level);
                await _context.SaveChangesAsync();
            }
        }
    }

}
