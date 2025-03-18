using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;
using BusinessObject.Models;

namespace Services.IService
{
    public interface IRequestService
    {
        Task<IEnumerable<RequestDto>> GetAllRequestsAsync(); // ✅ Trả về danh sách DTO
        Task<RequestDto> GetRequestByIdAsync(long requestId); // ✅ Trả về một DTO cụ thể
        Task<RequestDto> CreateRequestAsync(Guid userId, CreateRequestDto createRequestDto); // ✅ Nhận DTO đầu vào và trả về DTO
       // Task<RequestDto> UpdateRequestAsync(UpdateRequestDto updateRequestDto); // ✅ Cập nhật DTO
        Task<bool> ApproveRequestAsync(long requestId, Guid userId); // ✅ Trả về true nếu phê duyệt thành công
    }
}
