using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    /*public class ExportWarehouseReceiptRepository : IExportWarehouseReceiptRepository
    {
        private readonly MinhLongDbContext _context;

        public ExportWarehouseReceiptRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<ExportWarehouseReceipt> CreateExportWarehouseReceiptAsync(ExportWarehouseReceipt receipt)
        {
            _context.ExportWarehouseReceipts.Add(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }

        public async Task<ExportTransaction> CreateExportTransactionAsync(ExportTransaction transaction)
        {
            _context.ExportTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<ExportTransactionDetail> CreateExportTransactionDetailAsync(ExportTransactionDetail detail)
        {
            _context.ExportTransactionDetails.Add(detail);
            await _context.SaveChangesAsync();
            return detail;
        }
    }*/

}
