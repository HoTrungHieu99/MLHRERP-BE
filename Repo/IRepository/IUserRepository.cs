using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IUserRepository
    {
        Task<RegisterAccount> RegisterUserRequestAsync(RegisterAccount registerAccount);
        Task<RegisterAccount> GetRegisterAccountByIdAsync(int registerId);
        Task<bool> ApproveUserAsync(int registerId);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateUserAsync(User user);
    }


}
