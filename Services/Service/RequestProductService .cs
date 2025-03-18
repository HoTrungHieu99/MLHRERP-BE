using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using Services.IService;
using System.Linq;

namespace Services.Service
{
    public class RequestProductService : IRequestProductService
    {
        private readonly IRequestProductRepository _requestProductRepository;
        private readonly IOrderRepository _orderRepository;
        //private readonly IOrderDetailRepository _orderDetailRepository;

        public RequestProductService(
            IRequestProductRepository requestProductRepository,
            IOrderRepository orderRepository)
            //,IOrderDetailRepository orderDetailRepository)
        {
            _requestProductRepository = requestProductRepository;
            _orderRepository = orderRepository;
            //_orderDetailRepository = orderDetailRepository;
        }

        public async Task<IEnumerable<RequestProduct>> GetAllRequestsAsync()
        {
            return await _requestProductRepository.GetAllRequestsAsync();
        }

        public async Task<RequestProduct> GetRequestByIdAsync(Guid id)
        {
            return await _requestProductRepository.GetRequestByIdAsync(id);
        }

        public async Task CreateRequestAsync(RequestProduct requestProduct, List<RequestProductDetail> requestDetails)
        {
            var agencyId = requestProduct.AgencyId;

            // 🔹 Kiểm tra nếu đã có RequestProduct Pending của Agency
            var existingRequest = await _requestProductRepository.GetPendingRequestByAgencyAsync(agencyId);

            if (existingRequest != null)
            {
                foreach (var newItem in requestDetails)
                {
                    var existingDetail = existingRequest.RequestProductDetails
                        .FirstOrDefault(d => d.ProductId == newItem.ProductId);

                    if (existingDetail != null)
                    {
                        existingDetail.Quantity += newItem.Quantity; // ✅ Cập nhật số lượng nếu sản phẩm đã tồn tại
                    }
                    else
                    {
                        existingRequest.RequestProductDetails.Add(new RequestProductDetail
                        {
                            ProductId = newItem.ProductId,
                            Quantity = newItem.Quantity
                        });
                    }
                }

                await _requestProductRepository.UpdateRequestAsync(existingRequest);
            }
            else
            {
                // 🔹 Nếu không có đơn hàng Pending, tạo đơn mới
                requestProduct.CreatedAt = DateTime.UtcNow;
                requestProduct.RequestStatus = "Pending";
                requestProduct.RequestProductDetails = requestDetails;

                await _requestProductRepository.AddRequestAsync(requestProduct);
            }

            await _requestProductRepository.SaveChangesAsync();
        }

        public async Task ApproveRequestAsync(Guid requestId, long approvedBy)
        {
            try
            {
                var requestProduct = await _requestProductRepository.GetRequestByIdAsync(requestId);
                if (requestProduct == null) throw new Exception("Request not found!");

                // ✅ Kiểm tra nếu đơn hàng đã được duyệt trước đó
                if (requestProduct.RequestStatus == "Approved")
                {
                    throw new Exception("This request has already been approved and cannot be approved again.");
                }

                // **Cập nhật trạng thái RequestProduct**
                requestProduct.ApprovedBy = approvedBy;
                requestProduct.RequestStatus = "Approved";
                requestProduct.UpdatedAt = DateTime.UtcNow;

                await _requestProductRepository.UpdateRequestAsync(requestProduct);
                await _requestProductRepository.SaveChangesAsync(); // ✅ Lưu lại trạng thái RequestProduct

                // **Tạo Order từ RequestProduct**
                var order = new Order
                {
                    OrderDate = DateTime.UtcNow,
                    SalesAgentId = approvedBy,
                    Status = "Procesing",
                    RequestId = requestId,
                    Discount = 0,
                    FinalPrice = 0
                };

                await _orderRepository.AddOrderAsync(order);
                await _orderRepository.SaveChangesAsync(); // ✅ Lưu để lấy OrderId

                decimal finalPrice = 0;
                var orderDetails = new List<OrderDetail>();

                // **Tạo từng OrderDetail và tính tổng giá trị đơn hàng**
                foreach (var detail in requestProduct.RequestProductDetails)
                {
                    var unitPrice = 100; // 🔹 Lấy từ bảng Product nếu cần
                    var totalAmount = detail.Quantity * unitPrice;

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        UnitPrice = unitPrice,
                        TotalAmount = totalAmount,
                        CreatedAt = DateTime.UtcNow
                    };

                    finalPrice += totalAmount;
                    orderDetails.Add(orderDetail);
                }

                // ✅ Kiểm tra xem danh sách có rỗng không trước khi thêm vào database
                if (orderDetails.Count > 0)
                {
                    await _orderRepository.AddOrderDetailAsync(orderDetails); // ✅ Thêm danh sách OrderDetail
                }

                order.FinalPrice = finalPrice;

                await _orderRepository.UpdateOrderAsync(order); // ✅ Cập nhật tổng giá trị đơn hàng
                await _orderRepository.SaveChangesAsync();
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

    }
}
