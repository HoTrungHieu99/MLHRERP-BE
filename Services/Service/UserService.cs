using BusinessObject.DTO;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using Repo.Repository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        // ✅ Lưu yêu cầu đăng ký vào RegisterAccount
        public async Task<RegisterAccount> RegisterUserRequestAsync(RegisterRequest request)
        {
            // ✅ Kiểm tra Email hợp lệ
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
            {
                throw new ArgumentException("Invalid email! Must contain '@'!");
            }

            // ✅ Kiểm tra số điện thoại hợp lệ (10 chữ số, bắt đầu bằng '0')
            if (string.IsNullOrWhiteSpace(request.Phone) || !Regex.IsMatch(request.Phone, @"^0\d{9}$"))
            {
                throw new ArgumentException("Invalid phone number! Must be 10 digits and start with '0'.");
            }

            // ✅ Chuẩn hóa dữ liệu
            request.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.FullName.Trim().ToLower());
            request.Position = request.Position.Trim().ToUpper();
            request.Department = request.Department.Trim().ToUpper();
            request.UserType = request.UserType.Trim().ToUpper();
            request.Street = request.Street?.Trim();
            request.WardName = request.WardName?.Trim();
            request.DistrictName = request.DistrictName?.Trim();
            request.ProvinceName = request.ProvinceName?.Trim();

            // ✅ Kiểm tra giá trị hợp lệ
            if (request.Position != "STAFF" && request.Position != "MANAGER")
            {
                throw new ArgumentException("Position can only be 'STAFF' or 'MANAGER'!");
            }

            if (request.Department != "WAREHOUSE MANAGER" && request.Department != "SALES MANAGER")
            {
                throw new ArgumentException("Department can only be 'WAREHOUSE MANAGER' or 'SALES MANAGER'!");
            }

            if (request.UserType != "EMPLOYEE" && request.UserType != "AGENCY")
            {
                throw new ArgumentException("UserType can only be 'EMPLOYEE' or 'AGENCY'!");
            }

            // ✅ Tạo đối tượng RegisterAccount
            var registerAccount = new RegisterAccount
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim(),
                Phone = request.Phone.Trim(),
                UserType = request.UserType,
                FullName = request.FullName,
                Position = request.Position,
                Department = request.Department,
                Street = request.Street,
                WardName = request.WardName,
                DistrictName = request.DistrictName,
                ProvinceName = request.ProvinceName,
                AgencyName = request.AgencyName?.Trim()
            };

            // ✅ Gọi Repo để lưu RegisterAccount
            return await _userRepository.RegisterUserRequestAsync(registerAccount);
        }

        // ✅ Duyệt tài khoản và chuyển dữ liệu từ RegisterAccount vào User
        public async Task<bool> ApproveUserAsync(int registerId)
        {
            // ✅ Gọi Repo để duyệt tài khoản
            return await _userRepository.ApproveUserAsync(registerId);
        }
    }

}
