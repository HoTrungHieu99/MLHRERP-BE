using BusinessObject.DTO.RequestExport;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class RequestExportService : IRequestExportService
    {
        private readonly IExportRepository _requestExportRepository;

        public RequestExportService(IExportRepository requestExportRepository)
        {
            _requestExportRepository = requestExportRepository;
        }

        public async Task<List<RequestExportDto>> GetAllRequestExportsAsync()
        {
            var requestExports = await _requestExportRepository.GetAllRequestExportsAsync();

            return requestExports.Select(re => new RequestExportDto
            {
                RequestExportId = re.RequestExportId,
                OrderId = re.OrderId,
                AgencyName = re.RequestedByAgency?.AgencyName ?? "Unknown", // 👈 Gán tên đại lý
                ApprovedByName = re.ApprovedByEmployee?.FullName ?? "Chưa duyệt",
                Status = re.Status,
                ApprovedDate = re.ApprovedDate,
                Note = re.Note,
                RequestExportCode = re.RequestExportCode,
                RequestExportDetails = re.RequestExportDetails.Select(red => new RequestExportDetailDto
                {
                    RequestExportDetailId = red.RequestItemId,
                    ProductId = red.ProductId,
                    ProductName = red.Product?.ProductName ?? "N/A",
                    Unit = red.Product?.Unit ?? "N/A",
                    Price = red.Product?.Price ?? 0, // hoặc giá khác nếu có
                    RequestedQuantity = red.RequestedQuantity
                }).ToList()

            }).ToList();
        }

        public async Task<RequestExportDto> GetRequestExportByIdAsync(int requestId)
        {
            var requestExport = await _requestExportRepository.GetRequestExportById(requestId);

            if (requestExport == null)
            {
                return null; // hoặc throw exception nếu cần
            }

            return new RequestExportDto
            {
                RequestExportId = requestExport.RequestExportId,
                OrderId = requestExport.OrderId,
                AgencyName = requestExport.RequestedByAgency?.AgencyName ?? "Unknown", // 👈 Gán tên đại lý
                ApprovedByName = requestExport.ApprovedByEmployee?.FullName ?? "Chưa duyệt",
                Status = requestExport.Status,
                ApprovedDate = requestExport.ApprovedDate,
                Note = requestExport.Note,
                RequestExportCode = requestExport.RequestExportCode,
                RequestExportDetails = requestExport.RequestExportDetails != null
                    ? requestExport.RequestExportDetails.Select(red => new RequestExportDetailDto
                    {
                        RequestExportDetailId = red.RequestItemId,
                        ProductId = red.ProductId,
                        ProductName = red.Product?.ProductName ?? "N/A",
                        Unit = red.Product?.Unit ?? "N/A",
                        Price = red.Product?.Price ?? 0, // hoặc giá khác nếu có
                        RequestedQuantity = red.RequestedQuantity
                    }).ToList()
                    : new List<RequestExportDetailDto>() // Trả về list rỗng nếu null
            };
        }


    }
}
