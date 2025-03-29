﻿using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IOrderRepository
    {

        //Task<IEnumerable<Order>> GetAllOrdersAsync();

        Task<List<Order>> GetAllOrdersAsync();
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task SaveChangesAsync();

        Task AddOrderDetailAsync(List<OrderDetail> orderDetails);

        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<RequestProduct> GetRequestProductByOrderAsync(Guid orderId);
        Task SaveAsync();

        Task<int> GetTotalQuantityByOrderIdAsync(Guid orderId);

        Task<List<Order>> GetOrdersByAgencyIdAsync(long agencyId);

        Task<Order?> SingleOrDefaultAsync(Expression<Func<Order, bool>> predicate);

        Task<Order> GetOrderByOrderCodeAsync(string orderCode);

        Task<Order?> GetOrderByRequestIdAsync(Guid requestId);
        Task<OrderDetail?> GetOrderDetailAsync(Guid orderId, long productId);
        Task UpdateOrderDetailAsync(OrderDetail detail);
        Task<string> GenerateRequestExportCodeAsync();
        
    }
}
