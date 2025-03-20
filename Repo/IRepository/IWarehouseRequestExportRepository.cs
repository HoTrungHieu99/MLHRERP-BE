using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repo.IRepository
{
    public interface IWarehouseRequestExportRepository
    {
        Task<WarehouseRequestExport> CreateWarehouseRequestExportAsync(long warehouseId, int requestExportId);
        Task<RequestExport> GetRequestExportByIdAsync(int requestExportId);
    }
}
