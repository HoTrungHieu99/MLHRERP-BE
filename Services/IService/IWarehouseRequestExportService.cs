using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Services.IService
{
    public interface IWarehouseRequestExportService
    {
        Task<WarehouseRequestExport> CreateWarehouseRequestExportAsync(long warehouseId, int requestExportId);
    }
}
