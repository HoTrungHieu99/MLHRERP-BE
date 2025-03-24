using BusinessObject.DTO.PaymentDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repo.IRepository;
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

        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
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

        [HttpGet("Payment-confirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            if (!Request.Query.ContainsKey("orderid"))
                return Redirect("https://minhlong.mlhr.org/api/Payment/payment-fail");

            try
            {
                Guid orderId = Guid.Parse(Request.Query["orderid"]!);
                decimal amount = decimal.Parse(Request.Query["amount"]!);
                string accountId = Request.Query["accountId"]!;

                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                    throw new Exception("Không tìm thấy đơn hàng.");

                // ✅ Truy ngược lại orderCode để xác nhận thanh toán
                long orderCode = order.OrderCode;

                var queryRequest = new QueryRequest
                {
                    userId = accountId,
                    OrderId = orderId,
                    price = amount,
                    Paymentlink = orderCode.ToString()
                };

                var result = await _paymentService.ConfirmPayment(Request.QueryString.Value!, queryRequest);
                string formattedAmount = $"{amount:N0} VND";

                if (result != null && result.code == "00")
                {
                    return Content($@"
                <html><head><meta charset='UTF-8'><title>Thành công</title></head>
                <body style='text-align:center;font-family:sans-serif'>
                <h1 style='color:green'>THÀNH CÔNG RÙI NÈ</h1>
                <p>Số tiền: {formattedAmount}</p>
                <p>Cảm ơn bạn đã thanh toán!</p></body></html>", "text/html");
                }

                return Redirect("https://minhlong.mlhr.org/api/Payment/payment-fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xác nhận callback: " + ex.Message);
                return Redirect("https://minhlong.mlhr.org/api/Payment/payment-fail");
            }
        }


        // ✅ Trang báo thất bại
        [HttpGet("payment/fail")]
        public IActionResult PaymentFail()
        {
            return Content($@"
            <html><head><meta charset='UTF-8'><title>Thất bại</title></head>
            <body style='text-align:center;font-family:sans-serif'>
            <h1 style='color:red'>THẤT BẠI RÙI NÈ</h1>
            <p>Giao dịch không thành công hoặc dữ liệu phản hồi không hợp lệ.</p>
            <p>Xin vui lòng thử lại hoặc liên hệ hỗ trợ.</p></body></html>", "text/html");
        }


    }
}
