using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;
using BusinessObject.Models;

namespace Services.IService
{
    public interface IRequestProductService
    {
        Task<IEnumerable<RequestProduct>> GetAllRequestsAsync();
        Task<RequestProduct> GetRequestByIdAsync(Guid id);
        Task CreateRequestAsync(RequestProduct requestProduct, List<RequestProductDetail> requestDetails, Guid userId);
        Task ApproveRequestAsync(Guid requestId, long approvedBy);

        Task<List<RequestProduct>> GetRequestProductsByAgencyIdAsync(long agencyId);

        Task<bool> CancelRequestAsync(Guid requestId, long approvedBy);
    }
}
