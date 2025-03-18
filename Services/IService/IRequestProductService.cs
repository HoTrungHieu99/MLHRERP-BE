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
        Task<RequestProduct> GetRequestByIdAsync(int id);
        Task CreateRequestAsync(RequestProduct requestProduct, List<RequestProductDetail> requestDetails);
        Task ApproveRequestAsync(int requestId, int approvedBy);
        Task CreateOrUpdateRequestAsync(CreateRequestProductDto requestDto, int agencyId);
    }
}
