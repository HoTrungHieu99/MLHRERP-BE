using BusinessObject.Models;
using DataAccessLayer;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    public class ExportRepository : IExportRepository
    {
        private readonly MinhLongDbContext _context;

        public ExportRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task AddExportAsync(RequestExport export)
        {
            await _context.RequestExports.AddAsync(export);
        }

        public async Task AddExportDetailsAsync(List<RequestExportDetail> exportDetails)
        {
            await _context.RequestExportDetails.AddRangeAsync(exportDetails);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }


}
