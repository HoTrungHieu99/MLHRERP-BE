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
        Task<RequestProduct> GetPendingRequestByAgencyAsync(long agencyId);
        Task<bool> HasApprovedRequestInLast24Hours(long agencyId);
        Task<RequestProduct> GetRequestByIdAsync(Guid id);
        Task AddRequestAsync(RequestProduct requestProduct);
        Task UpdateRequestAsync(RequestProduct requestProduct);
        Task SaveChangesAsync();
        Task<RequestProduct> GetRequestProductByRequestIdAsync(Guid requestId);

    }
}
