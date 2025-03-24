﻿using System;
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
using Repo.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Services.Exceptions;
using MailKit.Search;

namespace Services.Service
{
    public class RequestProductService : IRequestProductService
    {
        private readonly IRequestProductRepository _requestProductRepository;
        private readonly IOrderRepository _orderRepository;
        //private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBatchRepository _batchRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public RequestProductService(
            IRequestProductRepository requestProductRepository,
            IOrderRepository orderRepository,
            IBatchRepository batchRepository,
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _requestProductRepository = requestProductRepository;
            _orderRepository = orderRepository;
            _batchRepository = batchRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<RequestProduct>> GetAllRequestsAsync()
        {
            return await _requestProductRepository.GetAllRequestsAsync();
        }

        public async Task<RequestProduct> GetRequestByIdAsync(Guid id)
        {
            var requestProduct = await _requestProductRepository.GetRequestProductByRequestIdAsync(id);
            if (requestProduct == null)
            {
                throw new KeyNotFoundException($"RequestProduct with ID {id} not found.");
            }
            return requestProduct;
        }

        public async Task<List<RequestProduct>> GetRequestProductsByAgencyIdAsync(long agencyId)
        {
            return await _requestProductRepository.GetRequestProductAgencyIdAsync(agencyId);
        }

        public async Task<List<RequestProduct>> GetRequestProductsByIdAsync(Guid requestId)
        {
            return await _requestProductRepository.GetRequestProductByIdAsync(requestId);
        }

        public async Task CreateRequestAsync(RequestProduct requestProduct, List<RequestProductDetail> requestDetails, Guid userId)
        {
            /*long requestCodeID = Math.Abs(userId.GetHashCode()) % 1000000000;*/

            Random random = new Random();
            long requestCodeID = random.Next(1000000, 9999999); // Tạo số trong khoảng 100000 - 999999

            // ✅ Lấy AgencyId từ UserId (GUID)
            var agencyId = await _userRepository.GetAgencyIdByUserId(userId);
            if (agencyId == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy AgencyId từ User đang đăng nhập.");
            }

            /*// ✅ Kiểm tra nếu Agency đã có đơn hàng Approved trong 24 giờ qua
            bool hasRecentApprovedRequest = await _requestProductRepository.HasApprovedRequestInLast24Hours(agencyId.Value);
            if (hasRecentApprovedRequest)
            {
                throw new BadRequestException("Bạn đã có một đơn hàng được duyệt trong vòng 24 giờ qua. Vui lòng đợi trước khi tạo đơn hàng mới.");
            }*/

            var existingRequest = await _requestProductRepository.GetPendingRequestByAgencyAsync(agencyId.Value);

            foreach (var newItem in requestDetails)
            {
                // ✅ Lấy thông tin sản phẩm từ Product
                var product = await _productRepository.GetByIdAsync(newItem.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"ProductId {newItem.ProductId} không tồn tại.");
                }

                // ✅ Kiểm tra số lượng tồn kho
                if (newItem.Quantity > product.AvailableStock)
                {
                    throw new ArgumentException($"Sản phẩm {newItem.ProductId} không đủ hàng. Bạn chỉ có thể đặt tối đa {product.AvailableStock}.");
                }

                // ✅ Lấy giá từ Batch và tính giá bán (SellingPrice)
                var batch = await _batchRepository.GetLatestBatchByProductIdAsync(newItem.ProductId);
                if (batch == null)
                {
                    throw new ArgumentException($"Không tìm thấy lô hàng nào cho ProductId {newItem.ProductId}.");
                }

                decimal unitPrice = batch.SellingPrice ?? 0;

                if (existingRequest != null)
                {
                    var existingDetail = existingRequest.RequestProductDetails.FirstOrDefault(d => d.ProductId == newItem.ProductId);
                    if (existingDetail != null)
                    {
                        existingDetail.Quantity += newItem.Quantity;
                    }
                    else
                    {
                        existingRequest.RequestProductDetails.Add(new RequestProductDetail
                        {
                            ProductId = newItem.ProductId,
                            Quantity = newItem.Quantity,
                            Price = unitPrice,
                            Unit = newItem.Unit,
                        });
                    }
                }
                else
                {
                    if (requestProduct.RequestProductDetails == null)
                    {
                        requestProduct.RequestProductDetails = new List<RequestProductDetail>();
                    }

                    requestProduct.RequestProductDetails.Add(new RequestProductDetail
                    {
                        ProductId = newItem.ProductId,
                        Quantity = newItem.Quantity,
                        Unit = newItem.Unit,
                        Price = unitPrice
                    });
                }
            }

            if (existingRequest != null)
            {
                existingRequest.RequestCode = requestCodeID; // Gán requestCode cho đơn hàng đã tồn tại
                await _requestProductRepository.UpdateRequestAsync(existingRequest);
            }
            else
            {
                requestProduct.AgencyId = agencyId.Value; // Gán AgencyId từ User đăng nhập
                requestProduct.CreatedAt = DateTime.Now;
                requestProduct.RequestStatus = "Pending";
                await _requestProductRepository.AddRequestAsync(requestProduct);
            }
            requestProduct.RequestCode = requestCodeID;
            await _requestProductRepository.SaveChangesAsync();
        }


        /*public async Task ApproveRequestAsync(Guid requestId, long approvedBy)
        {
            try
            {
                var requestProduct = await _requestProductRepository.GetRequestByIdAsync(requestId);
                long requestOrderCode = Math.Abs(requestProduct.RequestProductId.GetHashCode()) % 10000000;

                if (requestProduct == null) throw new Exception("Request not found!");

                // ✅ Kiểm tra nếu đơn hàng đã được duyệt trước đó
                if (requestProduct.RequestStatus == "Approved")
                {
                    throw new Exception("This request has already been approved and cannot be approved again.");
                }

                // **Cập nhật trạng thái RequestProduct**
                requestProduct.ApprovedBy = approvedBy;
                requestProduct.RequestStatus = "Approved";
                requestProduct.UpdatedAt = DateTime.Now;

                await _requestProductRepository.UpdateRequestAsync(requestProduct);
                await _requestProductRepository.SaveChangesAsync(); // ✅ Lưu lại trạng thái RequestProduct

                // **Tạo Order từ RequestProduct**
                var order = new Order
                {
                    OrderCode = requestOrderCode,
                    OrderDate = DateTime.Now,
                    SalesAgentId = approvedBy,
                    Status = "WaitPaid",
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
                    var unitPrice = detail.Price; // 🔹 Lấy từ bảng Product nếu cần
                    var totalAmount = detail.Quantity * unitPrice;

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        UnitPrice = unitPrice,
                        TotalAmount = totalAmount,
                        Unit = detail.Unit,
                        CreatedAt = DateTime.Now
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
        }*/

        public async Task ApproveRequestAsync(Guid requestId, long approvedBy)
        {
            try
            {
                var requestProduct = await _requestProductRepository.GetRequestByIdAsync(requestId);
                if (requestProduct == null) throw new Exception("Request not found!");

                long requestOrderCode = Math.Abs(requestProduct.RequestProductId.GetHashCode()) % 10000000;

                if (requestProduct.RequestStatus == "Approved")
                {
                    throw new Exception("This request has already been approved and cannot be approved again.");
                }

                // Cập nhật trạng thái RequestProduct
                requestProduct.ApprovedBy = approvedBy;
                requestProduct.RequestStatus = "Approved";
                requestProduct.UpdatedAt = DateTime.Now;

                await _requestProductRepository.UpdateRequestAsync(requestProduct);
                await _requestProductRepository.SaveChangesAsync();

                var existingOrder = await _orderRepository.GetOrderByRequestIdAsync(requestId);

                Order order;
                bool isNewOrder = false;

                if (existingOrder == null || existingOrder.Status != "WaitPaid")
                {
                    // ✅ Tạo mới
                    order = new Order
                    {
                        OrderCode = requestOrderCode,
                        OrderDate = DateTime.Now,
                        SalesAgentId = approvedBy,
                        Status = "WaitPaid",
                        RequestId = requestId,
                        Discount = 0,
                        FinalPrice = 0
                    };

                    await _orderRepository.AddOrderAsync(order);
                    await _orderRepository.SaveChangesAsync();
                    isNewOrder = true;
                }
                else
                {
                    // ✅ Cập nhật đơn cũ
                    order = existingOrder;
                    order.OrderDate = DateTime.Now;
                }

                decimal finalPrice = 0;
                var orderDetails = new List<OrderDetail>();

                foreach (var detail in requestProduct.RequestProductDetails)
                {
                    var unitPrice = detail.Price;
                    var totalAmount = detail.Quantity * unitPrice;

                    var existingDetail = !isNewOrder
                        ? await _orderRepository.GetOrderDetailAsync(order.OrderId, detail.ProductId)
                        : null;

                    if (existingDetail != null)
                    {
                        // ✅ Cập nhật nếu đã có
                        existingDetail.Quantity = detail.Quantity;
                        existingDetail.UnitPrice = unitPrice;
                        existingDetail.TotalAmount = totalAmount;
                        existingDetail.Unit = detail.Unit;
                        existingDetail.CreatedAt = DateTime.Now;

                        await _orderRepository.UpdateOrderDetailAsync(existingDetail);
                    }
                    else
                    {
                        // ✅ Thêm mới
                        var newDetail = new OrderDetail
                        {
                            OrderId = order.OrderId,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                            UnitPrice = unitPrice,
                            TotalAmount = totalAmount,
                            Unit = detail.Unit,
                            CreatedAt = DateTime.Now
                        };

                        orderDetails.Add(newDetail);
                    }

                    finalPrice += totalAmount;
                }

                if (orderDetails.Any())
                {
                    await _orderRepository.AddOrderDetailAsync(orderDetails);
                }

                order.FinalPrice = finalPrice;
                await _orderRepository.UpdateOrderAsync(order);
                await _orderRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Database update failed: {ex.InnerException?.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}", ex);
            }
        }


        public async Task<bool> CancelRequestAsync(Guid requestId, long approvedBy)
        {
            var requestProduct = await _requestProductRepository.GetRequestByIdAsync(requestId);

            if (requestProduct == null)
                throw new Exception("RequestProduct not found!");

            if (requestProduct.RequestStatus == "Canceled")
                throw new Exception("RequestProduct is already canceled!");

            if (requestProduct.RequestStatus == "Approved")
                throw new Exception("Cannot cancel an approved request!");

            requestProduct.RequestStatus = "Canceled";
            requestProduct.ApprovedBy = approvedBy;
            requestProduct.UpdatedAt = DateTime.Now;

            await _requestProductRepository.UpdateRequestAsync(requestProduct);
            await _requestProductRepository.SaveChangesAsync();

            return true;
        }

    }
}
