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
        // ✅ Lưu yêu cầu đăng ký vào RegisterAccount
        Task<RegisterAccount> RegisterUserRequestAsync(RegisterAccount registerAccount);

        // ✅ Lấy thông tin RegisterAccount theo ID
        Task<RegisterAccount> GetRegisterAccountByIdAsync(int registerId);

        // ✅ Duyệt tài khoản và lưu vào bảng User + Employee hoặc Agency (thay LocationId bằng AddressId)
        Task<bool> ApproveUserAsync(int registerId);

        // ✅ Cập nhật User (bao gồm mật khẩu)
        Task<bool> UpdateUserAsync(User user);

        // ✅ Tìm User theo UserId
        Task<User> GetUserByIdAsync(Guid userId);

        //Login
        Task<User> LoginAsync(string email, string password);
        //Logout
        Task<User> GetUserByEmailAsync(string email);
    }

}
