using BusinessObject.DTO.PaymentDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.IService;
using Services.Service;

namespace MLHR.Controllers
{
    [ApiController]
    [Route("api")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
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

        [HttpGet("Payment-confirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            if (Request.Query.Count == 0)
                return Redirect("LINK_PHAN_HOI_KHONG_HOP_LE");

            try
            {
                string paymentlink = Request.Query["id"]!;
                string status = Request.Query["status"]!;
                string code = Request.Query["code"]!;
                string des = Request.Query["desc"]!;
                string accountid = Request.Query["accountId"]!;
                string amountStr = Request.Query["amount"]!;
                string orderCodeStr = Request.Query["ordercode"]!;
                string orderIdStr = Request.Query["orderid"]!;

                decimal price = decimal.TryParse(amountStr, out var parsedAmount) ? parsedAmount : 0;
                int orderCode = int.TryParse(orderCodeStr, out var parsedOrderCode) ? parsedOrderCode : 0;
                Guid orderId = Guid.TryParse(orderIdStr, out var parsedGuid) ? parsedGuid : Guid.Empty;

                var request = new QueryRequest
                {
                    userId = accountid,
                    Code = code,
                    des = des,
                    orderCode = orderCode,
                    OrderId = orderId,
                    Paymentlink = paymentlink,
                    price = price,
                    Status = status
                };

                var result = await _paymentService.ConfirmPayment(Request.QueryString.Value!, request);
                string formattedAmount = $"{price:N0} VND";

                if (result != null && request.Code == "00")
                {
                    var successHtml = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Payment Success</title>
                <style>
                    body {{ font-family: Arial, sans-serif; text-align: center; }}
                    .container {{ margin-top: 50px; }}
                    h1 {{ color: #4CAF50; font-size: 36px; font-weight: bold; }}
                    p {{ font-size: 18px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>THÀNH CÔNG RÙI NÈ</h1>
                    <p>Mã giao dịch: {request.Code}</p>
                    <p>Số tiền: {formattedAmount}</p>
                    <p>Cảm ơn bạn đã thanh toán!</p>
                </div>
            </body>
            </html>";
                    return Content(successHtml, "text/html");
                }
                else
                {
                    var failureHtml = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Payment Failed</title>
                <style>
                    body {{ font-family: Arial, sans-serif; text-align: center; }}
                    .container {{ margin-top: 50px; }}
                    h1 {{ color: #FF0000; font-size: 36px; font-weight: bold; }}
                    p {{ font-size: 18px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>THẤT BẠI RÙI NÈ</h1>
                    <p>Mã giao dịch: {request.Code}</p>
                    <p>Số tiền: {formattedAmount}</p>
                    <p>Xin vui lòng thử lại hoặc liên hệ hỗ trợ.</p>
                </div>
            </body>
            </html>";
                    return Content(failureHtml, "text/html");
                }
            }
            catch (Exception)
            {
                return Redirect("LINK_PHAN_HOI_KHONG_HOP_LE");
            }
        }

    }
}
