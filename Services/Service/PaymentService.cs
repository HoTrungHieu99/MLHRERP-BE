using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using Repo.IRepository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly PayOS _payOS;
        private readonly PayOSSettings _payOSSettings;
        private readonly IOrderRepository _orderRepository;

        // Constructor có đầy đủ các dependency
        public PaymentService(IOptions<PayOSSettings> payOSSettings,
                              IPaymentRepository paymentRepository,
                              IOrderRepository orderRepository)
        {
            // Kiểm tra nếu payOSSettings bị null
            _payOSSettings = payOSSettings?.Value ?? throw new ArgumentNullException(nameof(payOSSettings));

            // Kiểm tra nếu các giá trị trong _payOSSettings là null
            if (string.IsNullOrEmpty(_payOSSettings.ClientId) ||
                string.IsNullOrEmpty(_payOSSettings.ApiKey) ||
                string.IsNullOrEmpty(_payOSSettings.ChecksumKey))
            {
                throw new Exception("Cấu hình PayOS không hợp lệ. Vui lòng kiểm tra appsettings.json");
            }

            // Khởi tạo PayOS SDK
            _payOS = new PayOS(_payOSSettings.ClientId, _payOSSettings.ApiKey, _payOSSettings.ChecksumKey);

            // Inject repository
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<bool> ProcessPaymentAsync(Guid orderId, decimal paymentAmount)
        {
            // 1. Kiểm tra xem đơn hàng có tồn tại không
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception($"Không tìm thấy đơn hàng với ID: {orderId}");
            }

            // 2. Kiểm tra nếu paymentAmount hợp lệ
            if (paymentAmount <= 0 || paymentAmount > order.FinalPrice)
            {
                throw new Exception("Số tiền thanh toán không hợp lệ.");
            }

            // 3. Chuẩn bị dữ liệu thanh toán cho PayOS
            var items = new List<ItemData>
    {
        new ItemData("Thanh toán đơn hàng", 1, Convert.ToInt32(paymentAmount))
    };

            long orderCode = Math.Abs(orderId.GetHashCode()) % 1000000000;

            /*// 4. Kiểm tra trạng thái thanh toán trên PayOS
            string paymentStatusPayOs = await CheckPaymentStatusAsync(orderCode);

            if (paymentStatusPayOs == "PAID")
            {
                throw new Exception($"Đơn hàng đã được thanh toán trước đó.");
            }*/

            var paymentData = new PaymentData(orderCode, Convert.ToInt32(paymentAmount),
                                              "Thanh toán đơn hàng", items,
                                              "https://clone-ui-user.vercel.app/agency/orders",
                                              "https://clone-ui-user.vercel.app/agency/orders");

            // 4. Gọi PayOS để tạo link thanh toán
            var createPaymentResult = await _payOS.createPaymentLink(paymentData);

            // 5. Kiểm tra tổng số tiền đã thanh toán trước đó
            var previousPayments = await _paymentRepository.GetPaymentsByOrderIdAsync(orderId);
            decimal totalPaidBefore = previousPayments?.Sum(p => p.PaymentAmount) ?? 0;
            decimal remainingDebt = order.FinalPrice - (totalPaidBefore + paymentAmount);

            // 6. Xác định trạng thái thanh toán
            string paymentStatus = remainingDebt == 0 ? "FULL_PAID" :
                                   (paymentAmount > 0 ? "PARTIALLY_PAID" : "UNPAID");

            // 7. Lưu thông tin thanh toán vào PaymentHistory
            var paymentHistory = new PaymentHistory
            {
                OrderId = orderId,
                PaymentMethod = "PayOS",
                PaymentDate = DateTime.UtcNow,
                SerieNumber = null,
                Status = paymentStatus,
                DueDate =DateTime.UtcNow,
                PrePaymentId = null,
                RemainingDebtAmount = remainingDebt,
                PaymentAmount = paymentAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalAmountPayment = order.FinalPrice
            };

            await _paymentRepository.AddPaymentHistoryAsync(paymentHistory);

            // 8. Lưu giao dịch vào PaymentTransaction
            var paymentTransaction = new PaymentTransaction
            {
                PaymentHistoryId = paymentHistory.PaymentHistoryId,
                PaymentDate = DateTime.UtcNow,
                Amount = paymentAmount,
                PaymentStatus = paymentStatus,
                TransactionReference = createPaymentResult.paymentLinkId
            };

            await _paymentRepository.AddPaymentTransactionAsync(paymentTransaction);
            // 11. Luôn cập nhật trạng thái đơn hàng thành "PAID" sau khi có thanh toán
            order.Status = "PAID";
            await _orderRepository.UpdateOrderAsync(order);
            await _paymentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<string> GetPaymentQRCodeAsync(Guid orderId)
        {
            var order = await _paymentRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng");
            }

            long orderCode = Math.Abs(orderId.GetHashCode()) % 1000000000;
            return await _paymentRepository.GetPaymentDetailsAsync(orderCode);
        }

        public async Task<string> CheckPaymentStatusAsync(long orderCode)
        {
            var checkPaymentUrl = $"https://api.payos.vn/v1/payments/{orderCode}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_payOSSettings.ApiKey}");

            var response = await httpClient.GetAsync(checkPaymentUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var paymentInfo = JsonConvert.DeserializeObject<PayOSPaymentResponse>(responseContent);
                return paymentInfo.Status; // Trả về trạng thái thanh toán ("PENDING", "PAID", "CANCELED")
            }

            return "NOT_FOUND";
        }

    }
}
