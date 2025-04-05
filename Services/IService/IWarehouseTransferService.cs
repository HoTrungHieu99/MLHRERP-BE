using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.Warehouse;

namespace Services.IService
{
    public interface IWarehouseTransferService
    {
        Task<WarehouseTransferRequestDetailDto> CreateTransferRequestAsync(WarehouseTransferRequestCreateDto dto, Guid requestedBy);

        Task<List<WarehouseTransferRequestDetailDto>> GetAllAsync();
        Task<WarehouseTransferRequestDetailDto?> GetByIdAsync(long id);

        Task<bool> PlanTransferRequestAsync(long requestId, long sourceWarehouseId, Guid plannerId);
        Task<List<WarehouseTransferRequestDetailDto>> GetRequestsToExportAsync(long sourceWarehouseId);

        Task<WarehouseTransferRequestDetailDto> AutoCreateTransferRequestFromRemainingAsync(AutoCreateTransferRequestDto dto, Guid requestedBy);

    }
}
