using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repo.IRepository
{
    public interface IRequestProductRepository
    {
        Task<IEnumerable<RequestProduct>> GetAllRequestsAsync();
        Task<RequestProduct> GetPendingRequestByAgencyAsync(int agencyId);
        Task<bool> HasApprovedRequestInLast24Hours(int agencyId);
        Task<RequestProduct> GetRequestByIdAsync(int id);
        Task AddRequestAsync(RequestProduct requestProduct);
        Task UpdateRequestAsync(RequestProduct requestProduct);
        Task SaveChangesAsync();
    }
}
