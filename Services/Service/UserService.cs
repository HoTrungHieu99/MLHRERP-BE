using BusinessObject.DTO;
using BusinessObject.Models;
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

        // Lưu yêu cầu đăng ký vào RegisterAccount
 
    public async Task<RegisterAccount> RegisterUserRequestAsync(RegisterRequest request)
    {
        // ✅ Kiểm tra Email phải chứa '@'
        if (!request.Email.Contains("@"))
        {
            throw new ArgumentException("Invalid email! Must contain '@'!");
        }

        // ✅ Kiểm tra số điện thoại có đúng 10 chữ số, bắt đầu bằng '0'
        if (!Regex.IsMatch(request.Phone, @"^0\d{9}$"))
        {
            throw new ArgumentException("Invalid phone number! Must be 10 digits and start with '0'.");
        }

        // ✅ Chuyển FullName thành chữ cái đầu viết hoa (Ví dụ: "john doe" → "John Doe")
        request.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.FullName.ToLower());

        // ✅ Chuyển Position về chữ in hoa và kiểm tra giá trị hợp lệ
        request.Position = request.Position.ToUpper();
        if (request.Position != "STAFF" && request.Position != "MANAGER")
        {
            throw new ArgumentException("Position can only be selected as 'STAFF' or 'MANAGER'!");
        }

        // ✅ Chuyển Department về chữ in hoa và kiểm tra giá trị hợp lệ
        request.Department = request.Department.ToUpper();
        if (request.Department != "WAREHOUSE MANAGER" && request.Department != "SALES MANAGER")
        {
            throw new ArgumentException("Department can only select 'WAREHOUSE MANAGER' or 'SALES MANAGER'!");
        }

        // ✅ Chuyển UserType về chữ in hoa và kiểm tra giá trị hợp lệ
        request.UserType = request.UserType.ToUpper();
        if (request.UserType != "EMPLOYEE" && request.UserType != "AGENCY")
        {
            throw new ArgumentException("UserType can only be selected as 'EMPLOYEE' or 'AGENCY'!");
        }

        var registerAccount = new RegisterAccount
        {
            Username = request.Username,
            Email = request.Email,
            Phone = request.Phone,
            UserType = request.UserType, // ✅ UserType luôn in hoa
            FullName = request.FullName, // ✅ FullName chữ cái đầu viết hoa
            LocationId = request.LocationId,
            Position = request.Position, // ✅ Position luôn in hoa
            Department = request.Department, // ✅ Department luôn in hoa
            Address = request.Address,
            AgencyName = request.AgencyName
        };

        return await _userRepository.RegisterUserRequestAsync(registerAccount);
    }


    // Duyệt tài khoản và chuyển dữ liệu từ RegisterAccount vào User
    public async Task<bool> ApproveUserAsync(int registerId)
        {
            return await _userRepository.ApproveUserAsync(registerId);
        }
    }

}
