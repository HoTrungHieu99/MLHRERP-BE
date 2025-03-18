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
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;

        public RequestService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<IEnumerable<RequestDto>> GetAllRequestsAsync()
        {
            var requests = await _requestRepository.GetAllRequestsAsync();

            return requests.Select(r => new RequestDto
            {
                RequestId = r.RequestProductId,
                AgencyId = r.AgencyId,
                AgencyName = r.AgencyAccount?.AgencyName ?? "Unknown",
                RequestStatus = r.RequestStatus,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                Items = r.RequestProductDetails.Select(d => new RequestItemDto
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<RequestDto> GetRequestByIdAsync(long requestId)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId);
            if (request == null) return null;

            return new RequestDto
            {
                RequestId = request.RequestProductId,
                AgencyId = request.AgencyId,
                AgencyName = request.AgencyAccount?.AgencyName ?? "Unknown",
                RequestStatus = request.RequestStatus,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                Items = request.RequestProductDetails.Select(d => new RequestItemDto
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity
                }).ToList()
            };
        }

        public async Task<RequestDto> CreateRequestAsync(Guid userId, CreateRequestDto createRequestDto)
        {

            var agencyId = await _requestRepository.GetAgencyIdByUserIdAsync(userId);
            if (agencyId == null) return null;

            var request = new RequestProduct
            {
                AgencyId = agencyId.Value,
                RequestStatus = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RequestProductDetails = createRequestDto.Items.Select(i => new RequestProductDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var createdRequest = await _requestRepository.CreateRequestAsync(request);

            return new RequestDto
            {
                RequestId = createdRequest.RequestProductId,
                AgencyId = createdRequest.AgencyId,
                RequestStatus = createdRequest.RequestStatus,
                CreatedAt = createdRequest.CreatedAt,
                UpdatedAt = createdRequest.UpdatedAt,
                Items = createdRequest.RequestProductDetails.Select(d => new RequestItemDto
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity
                }).ToList()
            };
        }

        public async Task<bool> ApproveRequestAsync(long requestId, Guid userId)
        {
            return await _requestRepository.ApproveRequestAsync(requestId, userId);
        }
    }



}
