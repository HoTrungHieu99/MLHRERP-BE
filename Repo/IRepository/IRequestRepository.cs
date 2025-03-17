using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repo.IRepository
{
    public interface IRequestRepository
    {
        Task<IEnumerable<RequestProduct>> GetAllRequestsAsync();
        Task<RequestProduct> GetRequestByIdAsync(long requestId);
        Task<RequestProduct> CreateRequestAsync(RequestProduct request);
        Task<RequestProduct> UpdateRequestAsync(RequestProduct request);
        Task<bool> ApproveRequestAsync(long requestId, Guid userId); // 🔥 Sửa long thành Guid
        Task<long?> GetAgencyIdByUserIdAsync(Guid userId);
        Task<long?> GetEmployeeIdByUserIdAsync(Guid userId);
    }
}
