using BusinessObject.DTO;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IUserService
    {
        Task<RegisterAccount> RegisterUserRequestAsync(RegisterRequest request);
        Task<bool> ApproveUserAsync(int registerId);
    }


}
