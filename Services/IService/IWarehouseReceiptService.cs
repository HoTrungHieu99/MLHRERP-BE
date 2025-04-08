﻿using BusinessObject.DTO.Warehouse;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IWarehouseReceiptService
    {
        Task<bool> CreateReceiptAsync(WarehouseReceiptRequest request, Guid currentUserId);
        Task<bool> ApproveReceiptAsync(long id, Guid currentUserId);
        Task<List<WarehouseReceiptDTO>> GetAllReceiptsByWarehouseIdAsync(long warehouseId, string? sortBy = null);


        Task<WarehouseReceiptDTO?> GetWarehouseReceiptDTOIdAsync(long Id);

        Task<WarehouseReceipt> CreateReceiptFromTransferAsync(long transferRequestId, Guid currentUserId);

        Task<byte[]> ExportReceiptToPdfAsync(long id);
    }

}
