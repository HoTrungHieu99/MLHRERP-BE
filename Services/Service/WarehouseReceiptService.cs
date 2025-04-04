using BusinessObject.DTO.Product;
using BusinessObject.DTO.Warehouse;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repo.IRepository;
using Repo.Repository;
using Services.Exceptions;
using Services.IService;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class WarehouseReceiptService : IWarehouseReceiptService
    {
        private readonly IWarehouseReceiptRepository _repository;
        private readonly IBatchRepository _batchRepository; // ✅ Thêm Batch Repository để kiểm tra số lượng lô đã tạo
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseTransferRepository _warehouseTransferRepository;

        public WarehouseReceiptService(IWarehouseReceiptRepository repository, IBatchRepository batchRepository, IWarehouseRepository warehouseRepository, IWarehouseTransferRepository warehouseTransferRepository)
        {
            _repository = repository;
            _batchRepository = batchRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseTransferRepository = warehouseTransferRepository;
        }

        public async Task<bool> CreateReceiptAsync(WarehouseReceiptRequest request, Guid currentUserId)
        {
            var validateImportType = new HashSet<string>
    {
        "Nhập Sản Xuất",
        "Nhập trả hàng",
        "Nhập Mua",
        "Nhập bổ sung"
    };

            if (!validateImportType.Contains(request.ImportType))
            {
                throw new Exception("ImportType is invalid! Only accepted: Nhập Sản Xuất, Nhập trả hàng, Nhập Mua, Nhập bổ sung!");
            }

            // ✅ Lấy UserId của Warehouse
            var warehouseUserId = await _warehouseRepository.GetUserIdByWarehouseIdAsync(request.WarehouseId);

            // ✅ Kiểm tra quyền sở hữu kho hàng
            if (warehouseUserId != currentUserId)
            {
                throw new BadRequestException("Kho này không phải kho của bạn! Bạn không có quyền gì ở kho này.");
            }

            // ✅ Tạo Batch Code duy nhất
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            int batchCountToday = await _batchRepository.CountBatchesByDateAsync(DateTime.Now);
            string batchCode = $"BA{datePart}-{(batchCountToday + 1):D3}";

            // ✅ Tạo batch list
            var processedBatches = request.Batches.Select(b => new BatchResponseDto
            {
                BatchCode = batchCode, // ✅ Batch Code tự động
                ProductId = b.ProductId,
                Unit = b.Unit,
                Quantity = b.Quantity,
                UnitCost = b.UnitCost,
                TotalAmount = b.Quantity * b.UnitCost,
                Status = "PENDING",
                DateOfManufacture = b.DateOfManufacture // ✅ Lưu ngày sản xuất
            }).ToList();

            // ✅ Tính tổng số lượng và giá trị
            int totalQuantity = processedBatches.Sum(b => b.Quantity);
            decimal totalPrice = processedBatches.Sum(b => b.TotalAmount);

            // ✅ Chuyển danh sách batch thành JSON để lưu trữ
            string batchesJson = JsonConvert.SerializeObject(processedBatches, Formatting.Indented);

            var warehouseReceipt = new WarehouseReceipt
            {
                DocumentNumber = request.DocumentNumber,
                DocumentDate = DateTime.Now,
                WarehouseId = request.WarehouseId,
                ImportType = request.ImportType,
                Supplier = request.Supplier,
                DateImport = DateTime.Now,
                TotalQuantity = totalQuantity,
                TotalPrice = totalPrice,
                BatchesJson = batchesJson,
                //IsApproved = request.IsApproved,
            };

            return await _repository.AddAsync(warehouseReceipt);
        }

        public async Task<bool> ApproveReceiptAsync(long id, Guid currentUserId)
        {
            return await _repository.ApproveAsync(id, currentUserId);
        }


        public async Task<List<WarehouseReceiptDTO>> GetAllReceiptsByWarehouseIdAsync(long warehouseId, string? sortBy = null)
        {
            var receipts = await _repository.GetAllAsync();

            // Lọc theo warehouseId
            var filteredReceipts = receipts
                .Where(receipt => receipt.WarehouseId == warehouseId);

            // 👉 Xử lý sắp xếp theo yêu cầu
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "status":
                        filteredReceipts = filteredReceipts.OrderBy(r => r.IsApproved);
                        break;
                    case "dateimport_desc":
                        filteredReceipts = filteredReceipts.OrderByDescending(r => r.DateImport);
                        break;
                    case "dateimport_asc":
                        filteredReceipts = filteredReceipts.OrderBy(r => r.DateImport);
                        break;
                }
            }

            return filteredReceipts.Select(receipt => new WarehouseReceiptDTO
            {
                WarehouseReceiptId = receipt.WarehouseReceiptId,
                DocumentNumber = receipt.DocumentNumber,
                DocumentDate = receipt.DocumentDate,
                WarehouseId = receipt.WarehouseId,
                ImportType = receipt.ImportType,
                Supplier = receipt.Supplier,
                DateImport = receipt.DateImport,
                TotalQuantity = receipt.TotalQuantity,
                TotalPrice = receipt.TotalPrice,
                IsApproved = receipt.IsApproved,
                Batches = JsonConvert.DeserializeObject<List<BatchResponseDto>>(receipt.BatchesJson) ?? new List<BatchResponseDto>()
            }).ToList();
        }

        public async Task<WarehouseReceiptDTO?> GetWarehouseReceiptDTOIdAsync(long Id)
        {
            var receipts = await _repository.GetAllAsync();
            var receipt = receipts.FirstOrDefault(r => r.WarehouseReceiptId == Id);

            if (receipt == null)
            {
                return null; // Trả về null nếu không tìm thấy
            }

            return new WarehouseReceiptDTO
            {
                WarehouseReceiptId = receipt.WarehouseReceiptId,
                DocumentNumber = receipt.DocumentNumber,
                DocumentDate = receipt.DocumentDate,
                WarehouseId = receipt.WarehouseId,
                ImportType = receipt.ImportType,
                Supplier = receipt.Supplier,
                DateImport = receipt.DateImport,
                TotalQuantity = receipt.TotalQuantity,
                TotalPrice = receipt.TotalPrice,
                IsApproved = receipt.IsApproved,
                Batches = JsonConvert.DeserializeObject<List<BatchResponseDto>>(receipt.BatchesJson) ?? new List<BatchResponseDto>()
                
            };
        }

        public async Task<WarehouseReceipt> CreateReceiptFromTransferAsync(long transferRequestId, Guid currentUserId)
        {
            // 🔍 Lấy yêu cầu điều phối
            var transferRequest = await _warehouseTransferRepository.GetByIdAsync(transferRequestId)
                                        ?? throw new Exception("Không tìm thấy yêu cầu điều phối");

            // 🔐 Kiểm tra quyền tại kho đích
            var warehouseUserId = await _repository.GetUserIdOfWarehouseAsync(transferRequest.DestinationWarehouseId);
            if (warehouseUserId != currentUserId)
                throw new UnauthorizedAccessException("Bạn không có quyền tạo phiếu nhập cho kho này!");

            // 🔁 Lấy phiếu xuất được duyệt
            var exportReceipt = await _repository.GetApprovedExportReceiptByTransferIdAsync(transferRequestId)
                                    ?? throw new Exception("Không tìm thấy phiếu xuất đã duyệt cho yêu cầu điều phối này.");

            var sourceWarehouseId = exportReceipt.WarehouseId;

            // 🔄 Tạo key để lấy dữ liệu warehouse sản phẩm
            var batchProductPairs = exportReceipt.ExportWarehouseReceiptDetails
                .Select(d => new { d.ProductId, d.BatchNumber })
                .Distinct()
                .ToList();

            var warehouseProducts = await _warehouseRepository
                .GetByWarehouseIdAndBatchAsync(sourceWarehouseId,
                    batchProductPairs.Select(x => (x.ProductId, x.BatchNumber)));

            var warehouseProductDict = warehouseProducts
                .ToDictionary(x => (x.ProductId, x.Batch.BatchCode), x => x);

            // 🧠 Lấy thông tin sản phẩm
            var productIds = batchProductPairs.Select(p => p.ProductId).Distinct().ToList();
            var productInfos = await _warehouseRepository.GetProductsByIdsAsync(productIds); // Phương thức cần được thêm trong repository
            var productInfoDict = productInfos.ToDictionary(p => p.ProductId, p => p);

            // 📦 Tạo batches
            var batches = exportReceipt.ExportWarehouseReceiptDetails.Select(d =>
            {
                var key = (d.ProductId, d.BatchNumber);
                warehouseProductDict.TryGetValue(key, out var wp);
                productInfoDict.TryGetValue(d.ProductId, out var product);

                var dateOfManufacture = wp?.Batch?.DateOfManufacture ?? DateTime.Now;
                var defaultExpiration = product?.DefaultExpiration ?? 12; // tháng
                var expiryDate = dateOfManufacture.AddMonths(defaultExpiration);

                return new BatchResponseDto
                {
                    BatchCode = d.BatchNumber,
                    ProductId = d.ProductId,
                    ProductName = product?.ProductName ?? "Không rõ",
                    Unit = product?.Unit ?? "Chưa xác định",
                    Quantity = d.Quantity,
                    UnitCost = d.UnitPrice,
                    TotalAmount = d.Quantity * d.UnitPrice,
                    Status = "PENDING",
                    DateOfManufacture = dateOfManufacture,
                    ExpiryDate = expiryDate
                };
            }).ToList();

            // 📑 Tạo phiếu nhập
            var receipt = new WarehouseReceipt
            {
                DocumentNumber = $"IMP-TRANS-{DateTime.Now:yyyyMMddHHmmss}",
                DocumentDate = DateTime.Now,
                WarehouseId = transferRequest.DestinationWarehouseId,
                ImportType = "Nhập Điều Phối",
                Supplier = "Nội Bộ",
                DateImport = DateTime.Now,
                TotalQuantity = batches.Sum(b => b.Quantity),
                TotalPrice = batches.Sum(b => b.TotalAmount),
                BatchesJson = JsonConvert.SerializeObject(batches, Formatting.Indented),
                Note = $"Nhập theo phiếu xuất #{exportReceipt.DocumentNumber} từ yêu cầu điều phối #{transferRequestId}",
                IsApproved = false
            };

            // 💾 Lưu & duyệt phiếu nhập
            var createdReceipt = await _repository.CreateReceiptAsync(receipt);

            transferRequest.Status = "Completed";
            await _warehouseTransferRepository.UpdateAsync(transferRequest);

            await _repository.ApproveAsync(createdReceipt.WarehouseReceiptId, currentUserId);

            return createdReceipt;
        }



    }

}
