using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IExportRepository
    {
        Task AddExportAsync(RequestExport export);
        Task AddExportDetailsAsync(List<RequestExportDetail> exportDetails);
        Task SaveChangesAsync();
    }

}
