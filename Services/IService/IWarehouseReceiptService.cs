﻿using BusinessObject.DTO;
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
        Task<bool> CreateReceiptAsync(WarehouseReceiptRequest request);
        Task<bool> ApproveReceiptAsync(long id);
        Task<List<WarehouseReceipt>> GetAllReceiptsAsync();
    }

}
