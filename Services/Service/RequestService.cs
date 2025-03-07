using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;
using BusinessObject.Models;
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
                RequestId = r.RequestId,
                ProductId = r.ProductId,
                Quantity = r.Quantity,
                RequestStatus = r.RequestStatus,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,

                // ✅ Lấy thông tin Agency từ bảng AgencyAccount
                AgencyId = r.AgencyId,
                AgencyName = r.AgencyAccount != null ? r.AgencyAccount.AgencyName : "Unknown"
            }).ToList();
        }

        public async Task<RequestDto> GetRequestByIdAsync(long requestId)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId);
            if (request == null) return null;

            return new RequestDto
            {
                RequestId = request.RequestId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                RequestStatus = request.RequestStatus,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,

                // ✅ Lấy thông tin Agency từ bảng AgencyAccount
                AgencyId = request.AgencyId,
                AgencyName = request.AgencyAccount != null ? request.AgencyAccount.AgencyName : "Unknown"
            };
        }
        public async Task<RequestDto> CreateRequestAsync(Guid userId, CreateRequestDto createRequestDto)
            {
                var agencyId = await _requestRepository.GetAgencyIdByUserIdAsync(userId);
                if (agencyId == null) return null;

                var request = new Request
                {
                    AgencyId = agencyId.Value,
                    ProductId = createRequestDto.ProductId,
                    Quantity = createRequestDto.Quantity,
                    RequestStatus = "PENDING",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdRequest = await _requestRepository.CreateRequestAsync(request);

                return new RequestDto
                {
                    RequestId = createdRequest.RequestId,
                    ProductId = createdRequest.ProductId,
                    Quantity = createdRequest.Quantity,
                    RequestStatus = createdRequest.RequestStatus,
                    CreatedAt = createdRequest.CreatedAt,
                    UpdatedAt = createdRequest.UpdatedAt
                };
            }

            public async Task<RequestDto> UpdateRequestAsync(UpdateRequestDto updateRequestDto)
            {
                var request = await _requestRepository.GetRequestByIdAsync(updateRequestDto.RequestId);
                if (request == null) return null;

                request.Quantity = updateRequestDto.Quantity;
                request.RequestStatus = updateRequestDto.RequestStatus;
                request.UpdatedAt = DateTime.UtcNow;

                var updatedRequest = await _requestRepository.UpdateRequestAsync(request);

                return new RequestDto
                {
                    RequestId = updatedRequest.RequestId,
                    ProductId = updatedRequest.ProductId,
                    Quantity = updatedRequest.Quantity,
                    RequestStatus = updatedRequest.RequestStatus,
                    CreatedAt = updatedRequest.CreatedAt,
                    UpdatedAt = updatedRequest.UpdatedAt
                };
            }

            public async Task<bool> ApproveRequestAsync(long requestId, Guid userId)
            {
                return await _requestRepository.ApproveRequestAsync(requestId, userId);
            }
        }
    }
