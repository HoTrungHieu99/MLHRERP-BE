using BusinessObject.DTO.PaymentDTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repo.IRepository;
using Repo.Repository;
using Services.IService;
using Services.Service;

namespace MLHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentService paymentService, IOrderService orderService, IPaymentRepository paymentRepository)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _paymentRepository = paymentRepository;
        }


        [HttpPost]
        [Route("{userId:Guid}")]
        public async Task<IActionResult> SendPaymentLink(Guid userId, CreatePaymentRequest request)
        {
            var result = await _paymentService.SendPaymentLink(userId, request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        /*[HttpGet("Payment-confirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            if (Request.Query.Count == 0)
            {
                Console.WriteLine("❌ Không có query string.");
                return Redirect("https://minhlong.mlhr.org/api/Payment/payment-fail");
            }

            try
            {
                string status = Request.Query["status"]!;
                string code = Request.Query["code"]!;
                string des = Request.Query["desc"]!;
                string accountid = Request.Query["accountId"]!;
                string amountStr = Request.Query["amount"]!;
                string orderIdStr = Request.Query["orderid"]!;

                decimal price = decimal.TryParse(amountStr, out var parsedAmount) ? parsedAmount : 0;
                Guid orderId = Guid.TryParse(orderIdStr, out var parsedGuid) ? parsedGuid : Guid.Empty;

                var request = new QueryRequest
                {
                    userId = accountid,
                    Code = code,
                    des = des,
                    OrderId = orderId,
                    price = price,
                    Status = status
                };


                var result = await _paymentService.ConfirmPayment(Request.QueryString.Value!, request);
                string formattedAmount = $"{price:N0} VND";

                if (result != null && request.Code == "00")
                {
                    return Content($@"
            <html><head><meta charset='UTF-8'><title>Thành công</title></head>
            <body style='text-align:center;font-family:sans-serif'>
            <h1 style='color:green'>THÀNH CÔNG RÙI NÈ</h1>
            <p>Mã giao dịch: {request.Code}</p>
            <p>Số tiền: {formattedAmount}</p>
            <p>Cảm ơn bạn đã thanh toán!</p></body></html>", "text/html");
                }

                return Redirect("https://minhlong.mlhr.org/api/payment/fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERR: " + ex.Message);
                return Redirect("https://minhlong.mlhr.org/api/payment/fail");
            }
        }*/

        [HttpGet("paymentconfirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            if (!Request.Query.ContainsKey("orderCode"))
                return Redirect("https://minhlong.mlhr.org/api/Payment/payment-fail");

            try
            {
                // 🔹 B1. Lấy tham số từ query
                long orderCode = long.Parse(Request.Query["orderCode"]!);
                decimal amount = decimal.Parse(Request.Query["amount"]!);
                string accountId = Request.Query["accountId"]!;
                Guid orderId = Guid.Parse(Request.Query["orderId"]!);

                // 🔹 B3. Tìm đơn hàng liên kết với orderCode
                var order = await _orderService.GetOrderByIdAsync(orderId); // nếu bạn lưu orderCode trong bảng Order
                if (order == null)
                    throw new Exception("Không tìm thấy đơn hàng tương ứng với orderCode.");

                // 🔹 B2. Kiểm tra nếu Transaction đã xử lý rồi
                var existingTransaction = await _paymentRepository.GetTransactionByReferenceAsync(orderCode.ToString());
                var paymentHistory = await _paymentRepository.GetPaymentHistoryByOrderIdAsync(orderId);
                /*if (existingTransaction != null)
                {
                    // ✅ Đã xử lý rồi → Trả giao diện luôn
                    string formattedAmount = $"{amount:N0} VND";
                    return Content($@"
                <html><head><meta charset='UTF-8'><title>Thành công</title></head>
                <body style='text-align:center;font-family:sans-serif'>
                <h1 style='color:green'>BẠN ĐÃ THANH TOÁN THÀNH CÔNG ĐƠN HÀNG #{orderCode}</h1>
                <p>Số tiền Thanh Toán: {formattedAmount}</p>
                <p>Cảm ơn bạn đã thanh toán!</p></body></html>", "text/html");
                }*/

                if (existingTransaction != null)
                {
                    string formattedAmount = $"{amount:N0} VND";
                    // ✅ Đã xử lý rồi → Chuyển hướng đến trang giao diện thành công
                    return new JsonResult(new
                    {
                        success = true,
                        //message = "Thanh toán thành công",
                        orderCode = order.OrderCode,
                        amount = formattedAmount,
                        createDate = existingTransaction.PaymentDate,
                        serialNumber = paymentHistory.SerieNumber
                    });

                }




                // 🔹 B4. Chuẩn bị dữ liệu xác nhận
                var queryRequest = new QueryRequest
                {
                    userId = accountId,
                    price = amount,
                    Paymentlink = orderCode.ToString(),
                    orderCode = (int)orderCode,
                    OrderId = order.OrderId,
                    Url = Request.QueryString.Value!
                };

                var result = await _paymentService.ConfirmPayment(Request.QueryString.Value!, queryRequest);
                string formattedAmount2 = $"{amount:N0} VND";

                if (result != null && result.code == "00")
                {
                    /*return Content($@"
                <html><head><meta charset='UTF-8'><title>Thành công</title></head>
                <body style='text-align:center;font-family:sans-serif'>
                <h1 style='color:green'>BẠN ĐÃ THANH TOÁN THÀNH CÔNG ĐƠN HÀNG #{order.OrderCode}</h1>
                <p>Số tiền Thanh Toán: {formattedAmount2}</p>
                <p>Cảm ơn bạn đã thanh toán!</p></body></html>", "text/html");*/

                    return new JsonResult(new
                    {
                        success = true,
                        //message = "Thanh toán thành công",
                        orderCode = order.OrderCode,
                        amount = formattedAmount2,
                        createDate = existingTransaction.PaymentDate,
                        serialNumber = paymentHistory.SerieNumber
                    });


                }

                return Redirect("https://minhlong.mlhr.org/api/Payment/payment-fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi xác nhận thanh toán: " + ex.Message);
                return Redirect("https://minhlong.mlhr.org/api/Payment/payment-fail");
            }
        }


        // ✅ Trang báo thất bại
        [HttpGet("payment-fail")]
        public IActionResult PaymentFail()
        {

            return Content($@"
            <html><head><meta charset='UTF-8'><title>Thất bại</title></head>
            <body style='text-align:center;font-family:sans-serif'>
            <h1 style='color:red'>BẠN ĐÃ THANH TOÁN THẤT BẠI</h1>
            <p>Giao dịch không thành công hoặc dữ liệu phản hồi không hợp lệ.</p>
            <p>Xin vui lòng thử lại hoặc liên hệ hỗ trợ.</p></body></html>", "text/html");
        }

        [HttpGet("test-ping")]
        public IActionResult Ping()
        {
            return Ok("✅ GET thành công từ server!");
        }

    }
}
