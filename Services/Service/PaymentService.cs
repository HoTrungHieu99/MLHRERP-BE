using BusinessObject.DTO.PaymentDTO;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repo.IRepository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly PayOS _payOS;
        private readonly PayOSSettings _payOSSettings;
        private readonly IOrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IOrderService _orderService;
        private readonly HttpClient _client;

        // Constructor có đầy đủ các dependency
        public PaymentService(IOptions<PayOSSettings> payOSSettings,
                              IPaymentRepository paymentRepository,
                              IOrderRepository orderRepository,
                              IConfiguration configuration,
                              IUserRepository userRepository,
                              HttpClient client,
                              IOrderService orderService)
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

            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _client = client;
        }
        /*public async Task<CreatePaymentResult> SendPaymentLink(Guid accountId, CreatePaymentRequest request)
        {
            try
            {
                var order = await _orderRepository.SingleOrDefaultAsync(p => p.OrderId == request.OrderId);
                if (order == null)
                    throw new Exception("Không tìm thấy đơn hàng.");

                Guid orderId = order.OrderId;

                //string returnUrl = $"https://minhlong.mlhr.org/api/Payment/Payment-confirm?accountId={accountId}&amount={request.Price}&appointment={request.AgencyId}";
                string returnUrl = $"https://minhlong.mlhr.org/api/Payment/Payment-confirm" +
                                    $"?status=PAID" +
                                    $"&code=00" +
                                    $"&desc=Thanh+toan+thanh+cong" +
                                    $"&accountId={accountId}" +
                                    $"&amount={request.Price}" +
                                    $"&orderid={request.OrderId}";


                //var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>().SingleOrDefaultAsync(predicate: p => p.Id == accountId);
                var agency = await _userRepository.GetAgencyAccountByUserIdAsync(accountId);
                if (agency == null) throw new Exception("account not null!!");


                int amount = (int)request.Price;
                string currentTimeString = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                long orderCode = long.Parse(currentTimeString.Substring(currentTimeString.Length - 6));
                var description = request.Description;
                string? clientId = _configuration["PayOS:ClientId"];
                var apikey = _configuration["PayOS:APIKey"];
                var checksumkey = _configuration["PayOS:ChecksumKey"];
                var returnurlfail = _configuration["PayOS:ReturnUrlFail"];

                PayOS pos = new PayOS(clientId, apikey, checksumkey);

                var signatureData = new Dictionary<string, object>
                 {
                     { "amount", amount },
                     { "cancelUrl", returnurlfail},
                     { "description", description },
                     { "expiredAt", DateTimeOffset.Now.ToUnixTimeSeconds() },
                     { "orderCode", orderCode },
                     { "returnUrl", returnUrl}
                 };
                var sortedSignatureData = new SortedDictionary<string, object>(signatureData);
                var dataForSignature = string.Join("&", sortedSignatureData.Select(p => $"{p.Key}={p.Value}"));
                var signature = ComputeHmacSha256(dataForSignature, checksumkey);
                DateTimeOffset expiredAt = DateTimeOffset.Now.AddMinutes(10);

                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: amount,
                    description: description,
                    items: new List<ItemData>(), // Provide a list of items if needed
                    cancelUrl: returnurlfail,
                    returnUrl: returnUrl,
                    signature: signature,
                    buyerName: agency.AgencyName,
                    expiredAt: (int)DateTimeOffset.Now.AddMinutes(10).ToUnixTimeSeconds()
                );

                paymentData.items.Add(new ItemData(agency.AgencyName, 1, amount));
                var createPaymentResult = await pos.createPaymentLink(paymentData);
                string url = createPaymentResult.checkoutUrl;
                return createPaymentResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }*/

        public async Task<CreatePaymentResult> SendPaymentLink(Guid accountId, CreatePaymentRequest request)
        {
            try
            {
                var order = await _orderRepository.SingleOrDefaultAsync(p => p.OrderId == request.OrderId);
                if (order == null)
                    throw new Exception("Không tìm thấy đơn hàng.");

                // ✅ Dùng OrderCode đã có
                long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var agency = await _userRepository.GetAgencyAccountByUserIdAsync(accountId);
                if (agency == null) throw new Exception("Tài khoản không hợp lệ.");

                int amount = (int)request.Price;
                string description = request.Description;
                string? clientId = _configuration["PayOS:ClientId"];
                var apikey = _configuration["PayOS:APIKey"];
                var checksumkey = _configuration["PayOS:ChecksumKey"];
                var returnurlfail = _configuration["PayOS:ReturnUrlFail"];

                // ✅ returnUrl chỉ cần OrderId
                string returnUrl = $"http://localhost:5214/api/Payment/paymentconfirm" +
                //string returnUrl = $"https://minhlong.mlhr.org/api/Payment/paymentconfirm" +
                   $"?orderCode={orderCode}" +
                   $"&accountId={accountId}" +
                   $"&amount={request.Price}"+
                   $"&orderId={request.OrderId}";


                var signatureData = new Dictionary<string, object>
                {
                    { "amount", amount },
                    { "cancelUrl", returnurlfail },
                    { "description", description },
                    { "expiredAt", DateTimeOffset.Now.ToUnixTimeSeconds() },
                    { "orderCode", orderCode },
                    { "returnUrl", returnUrl }
                    };

                var sortedSignatureData = new SortedDictionary<string, object>(signatureData);
                var dataForSignature = string.Join("&", sortedSignatureData.Select(p => $"{p.Key}={p.Value}"));
                var signature = ComputeHmacSha256(dataForSignature, checksumkey);

                PayOS pos = new PayOS(clientId, apikey, checksumkey);

                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: amount,
                    description: description,
                    items: new List<ItemData> { new ItemData(agency.AgencyName, 1, amount) },
                    cancelUrl: returnurlfail,
                    returnUrl: returnUrl,
                    signature: signature,
                    buyerName: agency.AgencyName,
                    expiredAt: (int)DateTimeOffset.Now.AddMinutes(10).ToUnixTimeSeconds()
                );

                var createPaymentResult = await pos.createPaymentLink(paymentData);
                return createPaymentResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tạo liên kết thanh toán: {ex.Message}");
                throw;
            }
        }


        private string ComputeHmacSha256(string data, string checksumKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public async Task<StatusPayment> ConfirmPayment(string queryString, QueryRequest requestquery)
        {
            

            try
            {
                //// 🔍 1. Check nếu transaction đã tồn tại → không xử lý lại
                /*var existedTransaction = await _paymentRepository.GetTransactionByReferenceAsync(requestquery.Paymentlink.ToString());
                if (existedTransaction != null)
                {
                    return new StatusPayment
                    {
                        code = "00",
                        Data = new data
                        {
                            status = "PAID",
                            amount = existedTransaction.Amount
                        }
                    };
                }*/

                /*if (Guid.TryParse(requestquery.Paymentlink, out Guid referenceId))
                {
                    var existedTransaction = await _paymentRepository.GetTransactionByReferenceAsync(referenceId);
                    if (existedTransaction != null)
                    {
                        return new StatusPayment
                        {
                            code = "00",
                            Data = new data
                            {
                                status = "PAID",
                                amount = existedTransaction.Amount
                            }
                        };
                    }
                }
                else
                {
                    throw new Exception("PaymentLink không hợp lệ (không phải GUID)");
                }*/


                var getUrl = $"https://api-merchant.payos.vn/v2/payment-requests/{requestquery.Paymentlink}";


                Guid? userId = Guid.TryParse(requestquery.userId, out var accountGuid) ? accountGuid : (Guid?)null;
                var agency = await _userRepository.GetAgencyAccountByUserIdAsync(userId);

                // Gửi request đến PayOS
                var request = new HttpRequestMessage(HttpMethod.Get, getUrl);
                request.Headers.Add("x-client-id", _configuration["PayOS:ClientId"]);
                request.Headers.Add("x-api-key", _configuration["PayOS:APIKey"]);

                var response = await _client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Không gửi được yêu cầu tới PayOS.");

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JObject.Parse(responseContent);
                var status = responseObject["data"]?["status"]?.ToString();

                /*if (status == "PAID")
                    return null!;*/

                decimal paidAmount = requestquery.price;

                // B1. Lấy thông tin đơn hàng
                var order = await _orderRepository.SingleOrDefaultAsync(p => p.OrderId == requestquery.OrderId);
                if (order == null)
                    throw new Exception("Không tìm thấy đơn hàng.");

                decimal totalOrderAmount = order.FinalPrice;
                decimal newRemainingDebt = 0;

                // B2. Kiểm tra đã có PaymentHistory cho đơn này chưa
                var existingHistory = await _paymentRepository.GetPaymentHistoryByOrderIdAsync(order.OrderId);

                if (existingHistory != null)
                {
                    // Cộng dồn tiền thanh toán
                    existingHistory.PaymentAmount += paidAmount;

                    if (existingHistory.PaymentAmount == totalOrderAmount)
                    {
                        existingHistory.RemainingDebtAmount = 0;
                        existingHistory.Status = "PAID";
                    }
                    else
                    {
                        existingHistory.RemainingDebtAmount = totalOrderAmount - existingHistory.PaymentAmount;
                        existingHistory.Status = "PARTIALLY_PAID";
                    }

                    existingHistory.UpdatedAt = DateTime.Now;
                    await _paymentRepository.UpdatePaymentHistoryAsync(existingHistory);
                }
                else
                {
                    // Giao dịch đầu tiên
                    var statusFlag = paidAmount >= totalOrderAmount ? "PAID" : "PARTIALLY_PAID";
                    newRemainingDebt = paidAmount >= totalOrderAmount ? 0 : totalOrderAmount - paidAmount;

                    existingHistory = new PaymentHistory
                    {
                        OrderId = order.OrderId,
                        PaymentMethod = "PayOS",
                        PaymentDate = DateTime.Now,
                        Status = statusFlag,
                        TotalAmountPayment = totalOrderAmount,
                        RemainingDebtAmount = newRemainingDebt,
                        PaymentAmount = paidAmount,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,  
                        SerieNumber = $"SER-{DateTime.Now.Ticks}",
                        UserId = userId.Value
                    };

                    // ❗ KHÔNG gán PaymentHistoryId ở đây

                    await _paymentRepository.InsertPaymentHistoryAsync(existingHistory);
                    await _paymentRepository.SaveChangesAsync(); // 👈 Lúc này Id mới được sinh
                    await _orderService.ProcessPaymentAsync(order.OrderId);

                }

                // ✅ newHistory.PaymentHistoryId đã được sinh tự động, dùng được ở đây:
                var transaction = new PaymentTransaction
                {
                    PaymentHistoryId = existingHistory.PaymentHistoryId, // ✅ lấy từ EF sau khi lưu
                    PaymentDate = DateTime.UtcNow,
                    Amount = paidAmount,
                    PaymentStatus = "PAID",
                    TransactionReference = requestquery.Paymentlink
                };

                await _paymentRepository.InsertPaymentTransactionAsync(transaction);
                //order.Status = "Paid";
                await _paymentRepository.SaveChangesAsync();               

                return new StatusPayment
                {
                    code = "00",
                    Data = new data
                    {
                        status = "PAID",
                        amount = paidAmount
                    }
                };

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xác nhận thanh toán: " + ex.Message);
            }
        }

        public async Task<PaymentTransaction?> GetTransactionByReferenceAsync(Guid paymentHistoryId)
        {
            return await _paymentRepository.GetTransactionByReferenceAsync(paymentHistoryId);
        }

        public async Task<PaymentHistory?> GetPaymentHistoryByOrderIdAsync(Guid orderId)
        {
            return await _paymentRepository.GetPaymentHistoryByOrderIdAsync(orderId);
        }
    }
}
