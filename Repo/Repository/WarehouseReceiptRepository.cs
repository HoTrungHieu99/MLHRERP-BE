using BusinessObject.DTO;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    public class WarehouseReceiptRepository : IWarehouseReceiptRepository
    {
        private readonly MinhLongDbContext _context;

        public WarehouseReceiptRepository(MinhLongDbContext context)
        {
            _context = context;
        }


        public async Task<bool> AddAsync(WarehouseReceipt warehouseReceipt)
        {
            _context.WarehouseReceipts.Add(warehouseReceipt);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<WarehouseReceipt> GetByIdAsync(long id)
        {
            return await _context.WarehouseReceipts.FindAsync(id);

        }

        public async Task<bool> ApproveAsync(long id)
        {
            var warehouseReceipt = await _context.WarehouseReceipts.FindAsync(id);
            if (warehouseReceipt.IsApproved == true)
            {
                throw new Exception("The order was previously activated!");
            }
            else
            {

                // 🔥 Lấy WarehouseReceipt từ database
                var receipt = await _context.WarehouseReceipts
                    .Where(w => w.WarehouseReceiptId == id)
                    .Select(w => new
                    {
                        w.DocumentNumber,
                        w.DocumentDate,
                        w.WarehouseId,
                        w.ImportType,
                        w.Supplier,
                        w.DateImport,
                        w.Note,
                        w.TotalQuantity,  // ✅ Lấy TotalQuantity
                        w.TotalPrice,     // ✅ Lấy TotalPrice
                        w.BatchesJson
                    })
                    .FirstOrDefaultAsync();

                if (receipt == null) return false;

                // 🔥 Tạo ImportTransaction từ WarehouseReceipt
                var importTransaction = new ImportTransaction
                {
                    DocumentNumber = receipt.DocumentNumber,
                    DocumentDate = receipt.DocumentDate,
                    WarehouseId = receipt.WarehouseId,
                    TypeImport = receipt.ImportType,
                    Supplier = receipt.Supplier,
                    DateImport = receipt.DateImport,
                    Note = receipt.Note,
                };

                _context.ImportTransactions.Add(importTransaction);
                await _context.SaveChangesAsync(); // ✅ Lưu ImportTransaction trước khi dùng ID của nó

                // 🔥 Tạo hoặc lấy ImportTransactionDetail
                var importDetail = await _context.ImportTransactionDetails
                    .FirstOrDefaultAsync(d => d.ImportTransactionId == importTransaction.ImportTransactionId);

                if (importDetail == null)
                {
                    importDetail = new ImportTransactionDetail
                    {
                        ImportTransactionId = importTransaction.ImportTransactionId,
                        TotalQuantity = receipt.TotalQuantity, // ✅ Lấy TotalQuantity từ WarehouseReceipt
                        TotalPrice = receipt.TotalPrice,       // ✅ Lấy TotalPrice từ WarehouseReceipt
                    };

                    _context.ImportTransactionDetails.Add(importDetail);
                    await _context.SaveChangesAsync(); // ✅ Lưu vào database để có ID
                }

                // ✅ Lấy `ImportTransactionDetailId` chính xác để dùng cho Batch
                long importTransactionDetailId = importDetail.ImportTransactionDetailId;

                // 🔥 Chuyển đổi JSON thành danh sách batch
                var batches = JsonConvert.DeserializeObject<List<BatchRequest>>(receipt.BatchesJson);

                foreach (var batch in batches)
                {
                    var product = await _context.Products.FindAsync(batch.ProductId);
                    if (product == null)
                    {
                        throw new Exception($"Product with ID {batch.ProductId} not found.");
                    }


                    // 🔥 Tạo Batch từ WarehouseReceipt với `importTransactionDetailId` chính xác
                    var newBatch = new Batch
                    {
                        ImportTransactionDetailId = importTransactionDetailId, // ✅ Đảm bảo có giá trị hợp lệ
                        BatchCode = batch.BatchCode,
                        ProductId = batch.ProductId,
                        Unit = batch.Unit,
                        Quantity = batch.Quantity,
                        UnitCost = batch.UnitCost,
                        TotalAmount = batch.UnitCost * batch.Quantity,
                        DateOfManufacture = batch.DateOfManufacture,
                        ExpiryDate = batch.DateOfManufacture.AddDays(product.DefaultExpiration ?? 0),
                        Status = "CALCULATING_PRICE"
                    };

                    _context.Batches.Add(newBatch);
                    await _context.SaveChangesAsync(); // ✅ Lưu Batch sau khi có ImportTransactionDetailId


                    // ✅ Tạo mới Inventory nếu chưa có
                    var inventory = new WarehouseProduct
                    {
                        ProductId = batch.ProductId,
                        WarehouseId = receipt.WarehouseId,
                        BatchId = newBatch.BatchId,
                        ExpirationDate = newBatch.ExpiryDate,
                        Quantity = batch.Quantity,
                        Status = batch.Status
                    };

                    _context.WarehouseProduct.Add(inventory);

                    product.AvailableStock += batch.Quantity;

                    warehouseReceipt.IsApproved = true;
                    await _context.SaveChangesAsync();
                }

                return true;
            }

        }


        public async Task<List<WarehouseReceipt>> GetAllAsync()
        {
            return await _context.WarehouseReceipts.ToListAsync();
        }

    }

}
