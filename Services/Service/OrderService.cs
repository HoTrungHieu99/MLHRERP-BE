using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using Repo.Repository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IExportRepository _exportRepository;
        private readonly IRequestProductRepository _requestProductRepository;
        private readonly IUserRepository _userRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IExportRepository exportRepository,
            IRequestProductRepository requestProductRepository,
            IUserRepository agencyRepository)
        {
            _orderRepository = orderRepository;
            _exportRepository = exportRepository;
            _requestProductRepository = requestProductRepository;
            _userRepository = agencyRepository;
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        /*public async Task<bool> ProcessPaymentAsync(Guid orderId)
        {
            // 🔹 Lấy Order từ OrderId
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null || order.Status != "Procesing")
                throw new Exception("Order not found or is not in a valid state.");

            // 🔹 Lấy RequestProduct từ RequestId của Order
            var requestProduct = await _requestProductRepository.GetRequestProductByRequestIdAsync(order.RequestId);
            if (requestProduct == null)
                throw new Exception("RequestProduct not found.");

            // ✅ Tính tổng số lượng từ OrderDetail có cùng OrderId
            int totalQuantity = await _orderRepository.GetTotalQuantityByOrderIdAsync(order.OrderId);


            // 🔹 Tạo RequestExport từ Order
            var requestExport = new RequestExport
            {
                RequestedBy = requestProduct.AgencyId,  // Gán AgencyId từ RequestProduct
                RequestDate = requestProduct.CreatedAt,
                Status = "Processing",
                ApprovedBy = requestProduct.ApprovedBy, // Lấy ApprovedBy từ RequestProduct
                ApprovedDate = requestProduct.UpdatedAt,
                Note = "Order approved and exported",
                OrderId = order.OrderId
            };

            // ✅ Lưu RequestExport vào database
            await _exportRepository.AddExportAsync(requestExport);
            await _exportRepository.SaveChangesAsync(); // 🔥 Lưu để lấy RequestExportId

            // 🔹 Lưu danh sách OrderDetail vào RequestExportDetail
            *//*var requestExportDetails = new List<RequestExportDetail>();

            foreach (var orderDetail in order.OrderDetails)
            {
                var requestExportDetail = new RequestExportDetail
                {
                    RequestExportId = requestExport.RequestExportId, // Liên kết với RequestExport
                    ProductId = orderDetail.ProductId,
                    RequestedQuantity = order.OrderDetails.Sum(od => od.Quantity)
                };

                requestExportDetails.Add(requestExportDetail);
            }*//*

            var requestExportDetails = order.OrderDetails
                .Where(od => od.OrderId == order.OrderId) // ✅ Chỉ lấy OrderDetail có cùng OrderId
                .Select(od => new RequestExportDetail
                 {
                        RequestExportId = requestExport.RequestExportId,
                        ProductId = od.ProductId,
                        RequestedQuantity = od.Quantity
                }).ToList();

            // ✅ Lưu danh sách RequestExportDetail vào database
            await _exportRepository.AddExportDetailsAsync(requestExportDetails);
            await _exportRepository.SaveChangesAsync();

            // 🔹 Cập nhật trạng thái đơn hàng
            order.Status = "Paid";
            await _orderRepository.UpdateOrderAsync(order);
            await _orderRepository.SaveChangesAsync();

            return true;
        }
*/

        public async Task<bool> ProcessPaymentAsync(Guid orderId)
        {
            try
            {
                // ✅ Lấy Order từ OrderId
                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                if (order == null || order.Status != "WaitPaid")
                    throw new Exception("Order not found or is not in a valid state.");

                // ✅ Lấy RequestProduct từ RequestId của Order
                var requestProduct = await _requestProductRepository.GetRequestProductByRequestIdAsync(order.RequestId);
                if (requestProduct == null)
                    throw new Exception("RequestProduct not found.");

                // ✅ Lấy `AgencyId` từ RequestProduct (RequestBy)
                long requestBy = requestProduct.AgencyId; // ✅ Lưu vào RequestExport.RequestedBy
/*
                // ✅ Lấy UserId từ JWT Token
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User ID not found in token.");*/

                // ✅ Lấy EmployeeId từ UserId thông qua UserRepository
                var employeeId = requestProduct.ApprovedBy;
                if (employeeId == null)
                    throw new Exception("Employee not found for the logged-in user.");

                long approvedBy = employeeId.Value; // ✅ Lưu vào RequestExport.ApprovedBy

                // ✅ Tạo RequestExport từ Order
                var requestExport = new RequestExport
                {
                    RequestedByAgencyId = requestBy,  // ✅ Lấy AgencyId từ RequestProduct
                    RequestDate = requestProduct.CreatedAt,
                    Status = "WaitPaid",
                    ApprovedBy = approvedBy,  // ✅ Lấy EmployeeId từ User đăng nhập
                    ApprovedDate = DateTime.UtcNow,
                    Note = "Order approved and exported",
                    OrderId = order.OrderId
                };

                // ✅ Lưu RequestExport vào database
                await _exportRepository.AddExportAsync(requestExport);
                await _exportRepository.SaveChangesAsync(); // 🔥 Lưu để lấy RequestExportId

                // ✅ Lấy danh sách OrderDetails từ OrderId và lưu vào RequestExportDetail
                var requestExportDetails = order.OrderDetails
                    .Select(od => new RequestExportDetail
                    {
                        RequestExportId = requestExport.RequestExportId,
                        ProductId = od.ProductId,
                        RequestedQuantity = od.Quantity
                    }).ToList();

                // ✅ Lưu danh sách RequestExportDetail vào database
                await _exportRepository.AddExportDetailsAsync(requestExportDetails);
                await _exportRepository.SaveChangesAsync();

                // ✅ Cập nhật trạng thái đơn hàng
                order.Status = "Paid";
                await _orderRepository.UpdateOrderAsync(order);
                await _orderRepository.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex) // ✅ Bắt lỗi từ Entity Framework
            {
                throw new Exception($"Database update failed: {ex.InnerException?.Message}", ex);
            }
            catch (Exception ex) // ✅ Bắt lỗi tổng quát
            {
                throw new Exception($"An error occurred: {ex.Message}", ex);
            }
        }


        public async Task<bool> CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            var requestProduct = await _requestProductRepository.GetRequestByIdAsync(order.RequestId);
            if (order == null || order.Status == "Paid") return false;
            
            if(order.Status == "WaitPaid")
            {
                order.Status = "Canceled";
                requestProduct.RequestStatus = "Canceled";
                await _orderRepository.UpdateOrderAsync(order);
               await _orderRepository.SaveAsync();
            }
            return true;
        }

        public async Task<List<Order>> GetOrdersByAgencyIdAsync(long agencyId)
        {
            return await _orderRepository.GetOrdersByAgencyIdAsync(agencyId);
        }



        
    }

}
