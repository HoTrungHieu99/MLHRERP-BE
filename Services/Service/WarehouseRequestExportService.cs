using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;

namespace Services.Service
{
    public class WarehouseRequestExportService : IWarehouseRequestExportService
    {
        private readonly IWarehouseRequestExportRepository _repository;

        public WarehouseRequestExportService(IWarehouseRequestExportRepository repository)
        {
            _repository = repository;
        }

        public async Task<WarehouseRequestExport> CreateWarehouseRequestExportAsync(long warehouseId, int requestExportId)
        {
            return await _repository.CreateWarehouseRequestExportAsync(warehouseId, requestExportId);
        }
    }
}
