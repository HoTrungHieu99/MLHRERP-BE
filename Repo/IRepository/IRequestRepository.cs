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
        Task<IEnumerable<Request>> GetAllRequestsAsync();
        Task<Request> GetRequestByIdAsync(long requestId);
        Task<Request> CreateRequestAsync(Request request);
        Task<Request> UpdateRequestAsync(Request request);
        Task<bool> ApproveRequestAsync(long requestId, Guid userId); // 🔥 Sửa long thành Guid
        Task<long?> GetAgencyIdByUserIdAsync(Guid userId);
        Task<long?> GetEmployeeIdByUserIdAsync(Guid userId);
    }
}
