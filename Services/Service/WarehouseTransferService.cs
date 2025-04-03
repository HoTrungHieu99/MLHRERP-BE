using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.Warehouse;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;

namespace Services.Service
{
    public class WarehouseTransferService : IWarehouseTransferService
    {
        private readonly IWarehouseTransferRepository _repository;

        public WarehouseTransferService(IWarehouseTransferRepository repository)
        {
            _repository = repository;
        }

        public async Task<WarehouseTransferRequestDetailDto> CreateTransferRequestAsync(WarehouseTransferRequestCreateDto dto, Guid requestedBy)
        {
            // ✳️ Gọi repo thay vì DbContext trực tiếp
            var requestExport = await _repository.GetRequestExportWithOrderAsync(dto.RequestExportId);
            if (requestExport == null)
                throw new Exception("RequestExport không tồn tại.");

            var orderCode = requestExport.Order?.OrderCode;

            var request = new WarehouseTransferRequest
            {
                RequestCode = $"REQ-{DateTime.UtcNow.Ticks}",
                SourceWarehouseId = dto.SourceWarehouseId,
                DestinationWarehouseId = dto.DestinationWarehouseId,
                ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
                RequestedBy = requestedBy,
                RequestDate = DateTime.UtcNow,
                Status = "Pending",
                Notes = dto.Notes,
                RequestExportId = dto.RequestExportId,
                OrderCode = orderCode,
                TransferProducts = dto.Products.Select(p => new WarehouseTransferProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    Unit = p.Unit,
                    Notes = p.Notes
                }).ToList()
            };

            var created = await _repository.CreateAsync(request);

            return new WarehouseTransferRequestDetailDto
            {
                Id = created.Id,
                RequestCode = created.RequestCode,
                SourceWarehouseId = created.SourceWarehouseId,
                DestinationWarehouseId = created.DestinationWarehouseId,
                RequestDate = created.RequestDate,
                Status = created.Status,
                Notes = created.Notes,
                OrderCode = created.OrderCode,
                Products = created.TransferProducts.Select(tp => new WarehouseTransferProductDto
                {
                    ProductId = tp.ProductId,
                    Quantity = tp.Quantity,
                    Unit = tp.Unit,
                    Notes = tp.Notes
                }).ToList()
            };
        }
        public async Task<List<WarehouseTransferRequestDetailDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();

            return list.Select(r => new WarehouseTransferRequestDetailDto
            {
                Id = r.Id,
                RequestCode = r.RequestCode,
                SourceWarehouseId = r.SourceWarehouseId,
                DestinationWarehouseId = r.DestinationWarehouseId,
                RequestDate = r.RequestDate,
                Status = r.Status,
                Notes = r.Notes,
                OrderCode = r.OrderCode,
                Products = r.TransferProducts.Select(p => new WarehouseTransferProductDto
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    Unit = p.Unit,
                    Notes = p.Notes
                }).ToList()
            }).ToList();
        }

        public async Task<WarehouseTransferRequestDetailDto?> GetByIdAsync(long id)
        {
            var r = await _repository.GetByIdAsync(id);
            if (r == null) return null;

            return new WarehouseTransferRequestDetailDto
            {
                Id = r.Id,
                RequestCode = r.RequestCode,
                SourceWarehouseId = r.SourceWarehouseId,
                DestinationWarehouseId = r.DestinationWarehouseId,
                RequestDate = r.RequestDate,
                Status = r.Status,
                Notes = r.Notes,
                OrderCode = r.OrderCode,
                Products = r.TransferProducts.Select(p => new WarehouseTransferProductDto
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    Unit = p.Unit,
                    Notes = p.Notes
                }).ToList()
            };
        }

        public async Task<bool> PlanTransferRequestAsync(long requestId, long sourceWarehouseId, Guid plannerId)
        {
            return await _repository.PlanTransferRequestAsync(requestId, sourceWarehouseId, plannerId);
        }

        public async Task<List<WarehouseTransferRequestDetailDto>> GetRequestsToExportAsync(long sourceWarehouseId)
        {
            var list = await _repository.GetPlannedRequestsByWarehouseAsync(sourceWarehouseId);

            return list.Select(r => new WarehouseTransferRequestDetailDto
            {
                Id = r.Id,
                RequestCode = r.RequestCode,
                SourceWarehouseId = r.SourceWarehouseId,
                DestinationWarehouseId = r.DestinationWarehouseId,
                RequestDate = r.RequestDate,
                Status = r.Status,
                Notes = r.Notes,
                OrderCode = r.OrderCode,
                Products = r.TransferProducts.Select(p => new WarehouseTransferProductDto
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    Unit = p.Unit,
                    Notes = p.Notes
                }).ToList()
            }).ToList();
        }

    }

}
