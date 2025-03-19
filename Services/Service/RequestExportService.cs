using BusinessObject.DTO;
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
                RequestedBy = re.RequestedByAgencyId,
                ApprovedBy = re.ApprovedBy ?? 0,
                Status = re.Status,
                ApprovedDate = re.ApprovedDate,
                Note = re.Note,
                RequestExportDetails = re.RequestExportDetails.Select(red => new RequestExportDetailDto
                {
                    RequestExportDetailId = red.RequestItemId,
                    ProductId = red.ProductId,
                    RequestedQuantity = red.RequestedQuantity
                }).ToList()
            }).ToList();
        }
    }
}
