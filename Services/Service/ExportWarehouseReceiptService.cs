using BusinessObject.DTO;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    /*public class ExportWarehouseReceiptService : IExportWarehouseReceiptService
    {
        private readonly IExportWarehouseReceiptRepository _repository;

        public ExportWarehouseReceiptService(IExportWarehouseReceiptRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExportWarehouseReceipt> CreateExportWarehouseReceiptAsync(ExportRequest exportRequest)
        {
            // Tạo phiếu xuất kho (ExportWarehouseReceipt)
            var receipt = new ExportWarehouseReceipt
            {
                DocumentNumber = exportRequest.DocumentNumber,
                DocumentDate = exportRequest.DocumentDate,
                ExportDate = exportRequest.ExportDate,
                ExportType = exportRequest.ExportType,
                WarehouseName = exportRequest.WarehouseName,
                ExportTransactionId = exportRequest.ExportTransactionId,
            };

            // Lưu phiếu xuất kho
            var createdReceipt = await _repository.CreateExportWarehouseReceiptAsync(receipt);

            // Tạo ExportTransaction (Giao dịch xuất kho)
            var transaction = new ExportTransaction
            {
                DocumentNumber = exportRequest.DocumentNumber,
                DocumentDate = exportRequest.DocumentDate,
                ExportDate = exportRequest.ExportDate,
                ExportType = exportRequest.ExportType,
                TotalAmount = exportRequest.ProductDetails.Sum(p => p.Quantity * p.UnitPrice)
            };

            // Lưu ExportTransaction
            var createdTransaction = await _repository.CreateExportTransactionAsync(transaction);

            // Lưu các chi tiết sản phẩm trong ExportTransactionDetail
            foreach (var product in exportRequest.ProductDetails)
            {
                var transactionDetail = new ExportTransactionDetail
                {
                    ExportTransactionId = createdTransaction.ExportTransactionId,
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    UnitPrice = product.UnitPrice,
                    TotalAmount = product.Quantity * product.UnitPrice
                };

                await _repository.CreateExportTransactionDetailAsync(transactionDetail);
            }

            return createdReceipt;
        }
    }*/

}
