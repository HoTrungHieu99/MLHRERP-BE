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
        private readonly PdfService _pdfService;

        public WarehouseReceiptService(IWarehouseReceiptRepository repository, IBatchRepository batchRepository, IWarehouseRepository warehouseRepository, IWarehouseTransferRepository warehouseTransferRepository, PdfService pdfService)
        {
            _repository = repository;
            _batchRepository = batchRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseTransferRepository = warehouseTransferRepository;
            _pdfService = pdfService;
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


        public async Task<byte[]> ExportReceiptToPdfAsync(long id)
        {
            var receipt = await GetWarehouseReceiptDTOIdAsync(id);
            if (receipt == null) throw new Exception("Không tìm thấy phiếu.");

            var productIds = receipt.Batches.Select(b => b.ProductId).Distinct().ToList();
            var products = await _warehouseRepository.GetProductsByIdsAsync(productIds);
            var productDict = products.ToDictionary(p => p.ProductId, p => p.ProductName);

            var html = new StringBuilder();
            html.AppendLine("<html><head><style>");
            html.AppendLine("body { font-family: Arial; font-size: 14px; }");
            html.AppendLine("table, th, td { border: 1px solid black; border-collapse: collapse; padding: 5px; }");
            html.AppendLine("th { background-color: #f2f2f2; }");
            html.AppendLine("</style></head><body>");

            html.AppendLine("<h2 style='text-align:center;'>PHIẾU NHẬP KHO</h2>");
            html.AppendLine($"<p><b>Số chứng từ:</b> {receipt.DocumentNumber}</p>");
            html.AppendLine($"<p><b>Ngày chứng từ:</b> {receipt.DocumentDate:dd/MM/yyyy}</p>");
            html.AppendLine($"<p><b>Ngày nhập thực tế:</b> {receipt.DateImport:dd/MM/yyyy}</p>");
            html.AppendLine($"<p><b>Kho:</b> {receipt.WarehouseId}</p>");
            html.AppendLine($"<p><b>Nhà cung cấp:</b> {receipt.Supplier}</p>");
            html.AppendLine($"<p><b>Loại nhập:</b> {receipt.ImportType}</p>");

            html.AppendLine("<h4>Danh sách lô hàng</h4>");
            html.AppendLine("<table width='100%'>");
            html.AppendLine("<tr><th>STT</th><th>Mã lô</th><th>Sản phẩm</th><th>SL</th><th>Đơn giá</th><th>Thành tiền</th><th>NSX</th></tr>");

            int stt = 1;
            foreach (var batch in receipt.Batches)
            {
                var productName = productDict.ContainsKey(batch.ProductId)
                    ? productDict[batch.ProductId]
                    : "Không rõ";

                html.AppendLine("<tr>");
                html.AppendLine($"<td>{stt++}</td>");
                html.AppendLine($"<td>{batch.BatchCode}</td>");
                html.AppendLine($"<td>{productName}</td>");
                html.AppendLine($"<td>{batch.Quantity}</td>");
                html.AppendLine($"<td>{batch.UnitCost:N0}</td>");
                html.AppendLine($"<td>{batch.TotalAmount:N0}</td>");
                html.AppendLine($"<td>{batch.DateOfManufacture:dd/MM/yyyy}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine($"<p><b>Tổng số lượng:</b> {receipt.TotalQuantity}</p>");
            html.AppendLine($"<p><b>Tổng tiền:</b> {receipt.TotalPrice:N0} VND</p>");

            html.AppendLine("<br/><br/><table width='100%'><tr>");
            html.AppendLine("<td style='text-align:center;'>Người lập phiếu<br/><br/><br/>[Ký tên]</td>");
            html.AppendLine("<td style='text-align:center;'>Người duyệt<br/><br/><br/>[Ký tên]</td>");
            html.AppendLine("</tr></table>");
            html.AppendLine("</body></html>");

            return _pdfService.GeneratePdf(html.ToString());
        }



    }

}
