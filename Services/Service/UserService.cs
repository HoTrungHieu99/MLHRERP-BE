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
            // ✅ Kiểm tra Email hợp lệ (chỉ khi có dữ liệu)
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
            {
                throw new ArgumentException("Invalid email! Must contain '@'!");
            }

            // ✅ Kiểm tra số điện thoại hợp lệ (chỉ khi có dữ liệu)
            if (string.IsNullOrWhiteSpace(request.Phone) || !Regex.IsMatch(request.Phone, @"^0\d{9}$"))
            {
                throw new ArgumentException("Invalid phone number! Must be 10 digits and start with '0'.");
            }

            // ✅ Kiểm tra UserType hợp lệ
            if (string.IsNullOrWhiteSpace(request.UserType) ||
                (request.UserType.ToUpper() != "EMPLOYEE" && request.UserType.ToUpper() != "AGENCY"))
            {
                throw new ArgumentException("UserType must be either 'EMPLOYEE' or 'AGENCY'!");
            }

            // ✅ Nếu UserType là EMPLOYEE -> Bắt buộc nhập FullName, Position, Department
            if (request.UserType.ToUpper() == "EMPLOYEE")
            {
                if (string.IsNullOrWhiteSpace(request.FullName))
                {
                    throw new ArgumentException("FullName is required for EMPLOYEE.");
                }
                if (string.IsNullOrWhiteSpace(request.Position) ||
                    (request.Position.ToUpper() != "STAFF" && request.Position.ToUpper() != "MANAGER"))
                {
                    throw new ArgumentException("Position must be 'STAFF' or 'MANAGER' for EMPLOYEE.");
                }
                if (string.IsNullOrWhiteSpace(request.Department) ||
                    (request.Department.ToUpper() != "WAREHOUSE MANAGER" && request.Department.ToUpper() != "SALES MANAGER"))
                {
                    throw new ArgumentException("Department must be 'WAREHOUSE MANAGER' or 'SALES MANAGER' for EMPLOYEE.");
                }

                // ✅ Nếu là Employee thì AgencyName có thể null
                request.AgencyName = "unknow";
            }

            // ✅ Nếu UserType là AGENCY -> Bắt buộc nhập AgencyName, các trường khác có thể null
            if (request.UserType.ToUpper() == "AGENCY")
            {
                if (string.IsNullOrWhiteSpace(request.AgencyName))
                {
                    throw new ArgumentException("AgencyName is required for AGENCY.");
                }

                // ✅ Nếu là Agency thì FullName, Position, Department có thể null
                request.FullName = "unknow";
                request.Position = "unknow";
                request.Department = "unknow";
            }

            // ✅ Chuẩn hóa dữ liệu (chỉ khi có giá trị)
            request.FullName = !string.IsNullOrWhiteSpace(request.FullName)
                ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.FullName.Trim().ToLower())
                : null;

            request.Position = !string.IsNullOrWhiteSpace(request.Position)
                ? request.Position.Trim().ToUpper()
                : null;

            request.Department = !string.IsNullOrWhiteSpace(request.Department)
                ? request.Department.Trim().ToUpper()
                : null;

            request.UserType = request.UserType.Trim().ToUpper();
            request.Street = request.Street?.Trim();
            request.WardName = request.WardName?.Trim();
            request.DistrictName = request.DistrictName?.Trim();
            request.ProvinceName = request.ProvinceName?.Trim();
            request.AgencyName = request.AgencyName?.Trim();

            // ✅ Tạo đối tượng RegisterAccount
            var registerAccount = new RegisterAccount
            {
                Username = request.Username?.Trim(),
                Email = request.Email?.Trim(),
                Phone = request.Phone?.Trim(),
                UserType = request.UserType,
                FullName = request.FullName,
                Position = request.Position,
                Department = request.Department,
                Street = request.Street,
                WardName = request.WardName,
                DistrictName = request.DistrictName,
                ProvinceName = request.ProvinceName,
                AgencyName = request.AgencyName
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
