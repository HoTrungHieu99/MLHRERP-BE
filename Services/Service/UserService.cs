using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> ApproveUserAsync(long userId)
        {
            // Tìm User trước
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User does not exist!");

            // Cập nhật trạng thái của User
            bool isUpdated = await _userRepository.UpdateUserStatusAsync(userId);
            if (!isUpdated) return false;

            // Lưu vào bảng Employee hoặc AgencyAccount dựa vào UserType
            await _userRepository.AddToEmployeeOrAgencyAsync(user);

            return true;
        }


        public async Task<User> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra nếu Email, Username hoặc Phone đã tồn tại
            if (await _userRepository.IsUsernameExistAsync(request.Username))
                throw new ArgumentException("Username already exists!");

            if (await _userRepository.IsEmailExistAsync(request.Email))
                throw new ArgumentException("Email already exists!");

            if (await _userRepository.IsPhoneExistAsync(request.Phone))
                throw new ArgumentException("Phone number already exists!");

            // Chuyển đổi từ RegisterRequest sang User
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password, // Hash mật khẩu trong repository
                UserType = request.UserType,
                Phone = request.Phone,
                Status = false // Mặc định chưa duyệt
            };

            // Lưu vào bảng User
            return await _userRepository.RegisterUserAsync(user);
        }


    }


}
