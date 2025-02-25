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

        //Login
        public async Task<User> LoginAsync(string email, string password)
        {
            return await _userRepository.LoginAsync(email, password);
        }

        //Logout
        public async Task<bool> LogoutAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            // ✅ Nếu bạn dùng Session hoặc Refresh Token, xóa token tại đây
            // Ví dụ: user.RefreshToken = null;
            await _userRepository.UpdateUserAsync(user);

            return true; // Trả về true nếu logout thành công
        }

        public async Task<bool> UpdateUserAccountAsync(Guid userId, UpdateUserRequest request)
        {
            // ✅ 1. Lấy thông tin User từ database
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            // ✅ 2. Cập nhật thông tin User (Email, Phone)
            if (!string.IsNullOrWhiteSpace(request.Email))
                user.Email = request.Email.Trim();

            if (!string.IsNullOrWhiteSpace(request.Phone))
                user.Phone = request.Phone.Trim();

            if (!string.IsNullOrWhiteSpace(request.Username))
                user.Username = request.Username.Trim();

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                // ✅ Kiểm tra độ dài phải đúng 9 ký tự
                if (request.Password.Length != 9)
                {
                    throw new ArgumentException("Password must be exactly 9 characters long.");
                }

                // ✅ Kiểm tra có ít nhất một ký tự đặc biệt
                if (!Regex.IsMatch(request.Password, @"[!@#$%^&*]"))
                {
                    throw new ArgumentException("Password must contain at least one special character (!@#$%^&*).");
                }

                // ✅ Kiểm tra có ít nhất một chữ cái viết hoa
                if (!Regex.IsMatch(request.Password, @"[A-Z]"))
                {
                    throw new ArgumentException("Password must contain at least one uppercase letter.");
                }

                // ✅ Hash mật khẩu trước khi lưu
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _userRepository.UpdateUserAsync(user); // ✅ Cập nhật bảng User

            int addressId = 0; // Lưu AddressId để cập nhật bảng Address

            // ✅ 3. Nếu UserType là EMPLOYEE → Cập nhật bảng Employee
            if (user.UserType.ToUpper() == "EMPLOYEE")
            {
                var employee = await _userRepository.GetEmployeeByUserIdAsync(userId);
                if (employee == null)
                    throw new ArgumentException("Employee record not found.");

                if (!string.IsNullOrWhiteSpace(request.FullName))
                    employee.FullName = request.FullName.Trim();

                // ✅ Lưu AddressId để cập nhật bảng Address
                addressId = employee.AddressId;

                await _userRepository.UpdateEmployeeAsync(employee);
            }

            // ✅ 4. Nếu UserType là AGENCY → Cập nhật bảng AgencyAccount
            else if (user.UserType.ToUpper() == "AGENCY")
            {
                var agency = await _userRepository.GetAgencyAccountByUserIdAsync(userId);
                if (agency == null)
                    throw new ArgumentException("Agency account not found.");

                if (!string.IsNullOrWhiteSpace(request.AgencyName))
                    agency.AgencyName = request.AgencyName.Trim();

                // ✅ Lưu AddressId để cập nhật bảng Address
                addressId = agency.AddressId;

                await _userRepository.UpdateAgencyAccountAsync(agency);
            }

            // ✅ 5. Cập nhật bảng Address nếu có AddressId
            if (addressId > 0 && !string.IsNullOrWhiteSpace(request.Street) &&
                !string.IsNullOrWhiteSpace(request.DistrictName) &&
                !string.IsNullOrWhiteSpace(request.ProvinceName) &&
                !string.IsNullOrWhiteSpace(request.WardName))
            {
                var address = await _userRepository.GetAddressByIdAsync(addressId);
                if (address == null)
                    throw new ArgumentException("Address not found.");

                var (province, district, ward) = await _userRepository.GetLocationIdsAsync(request.ProvinceName, request.DistrictName, request.WardName);

                address.ProvinceId = province.ProvinceId;
                address.DistrictId = district.DistrictId;
                address.WardId = ward.WardId;
                address.Street = request.Street.Trim();

                await _userRepository.UpdateAddressAsync(address);
            }

            return true;
        }
    }

}
