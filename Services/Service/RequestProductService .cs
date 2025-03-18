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

        public async Task<RequestProduct> GetRequestByIdAsync(int id)
        {
            return await _requestProductRepository.GetRequestByIdAsync(id);
        }

        public async Task CreateOrUpdateRequestAsync(CreateRequestProductDto requestDto, int agencyId)
        {
            // 🔹 Kiểm tra nếu Agency có đơn hàng `Approved` trong 24 giờ qua
            bool hasApprovedOrder = await _requestProductRepository.HasApprovedRequestInLast24Hours(agencyId);
            if (hasApprovedOrder)
            {
                throw new Exception("You must wait 24 hours after your last approved order before creating a new one.");
            }

            // 🔹 Kiểm tra nếu đã có RequestProduct Pending
            var existingRequest = await _requestProductRepository.GetPendingRequestByAgencyAsync(agencyId);

            if (existingRequest != null)
            {
                // 🔹 Cập nhật `RequestProductDetail` nếu sản phẩm đã có
                foreach (var newItem in requestDto.Products)
                {
                    var existingDetail = existingRequest.RequestProductDetails
                        .Find(d => d.ProductId == newItem.ProductId);

                    if (existingDetail != null)
                    {
                        existingDetail.Quantity += newItem.Quantity; // ✅ Cập nhật số lượng
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
                // 🔹 Tạo RequestProduct mới
                var newRequest = new RequestProduct
                {
                    AgencyId = agencyId,
                    RequestStatus = "Pending",
                    CreatedAt = DateTime.UtcNow, // ✅ Lưu `CreatedAt` ở RequestProduct
                    RequestProductDetails = new List<RequestProductDetail>()
                };

                foreach (var item in requestDto.Products)
                {
                    newRequest.RequestProductDetails.Add(new RequestProductDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }

                await _requestProductRepository.AddRequestAsync(newRequest);
            }

            await _requestProductRepository.SaveChangesAsync();
        }

        public async Task ApproveRequestAsync(int requestId, int approvedBy)
        {
            var requestProduct = await _requestProductRepository.GetRequestByIdAsync(requestId);
            if (requestProduct == null) throw new Exception("Request not found!");

            requestProduct.ApprovedBy = approvedBy;
            requestProduct.RequestStatus = "Approved";
            requestProduct.UpdatedAt = DateTime.UtcNow;

            await _requestProductRepository.UpdateRequestAsync(requestProduct);
            await _requestProductRepository.SaveChangesAsync();

            // **Bước 1: Tạo Order trước**
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                SalesAgentId = approvedBy,
                Status = "Pending",
                RequestId = requestId, // Gán RequestId vào Order
                Discount = 0,
                FinalPrice = 0
            };

            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync(); // Lưu để lấy OrderId

            decimal finalPrice = 0;

            // **Bước 2: Tạo từng OrderDetail và tính tổng giá trị đơn hàng**
            foreach (var detail in requestProduct.RequestProductDetails)
            {
                var unitPrice = 100; // Lấy từ bảng Product nếu cần
                var totalAmount = detail.Quantity * unitPrice;

                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId, // Sử dụng OrderId đã tạo
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitPrice = unitPrice,
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.UtcNow
                };

                finalPrice += totalAmount;

                await _orderRepository.AddOrderDetailAsync(orderDetail);
            }

            // **Cập nhật FinalPrice cho Order**
            order.FinalPrice = finalPrice;
            await _orderRepository.UpdateOrderAsync(order);
            await _orderRepository.SaveChangesAsync();
        }
    }
}
