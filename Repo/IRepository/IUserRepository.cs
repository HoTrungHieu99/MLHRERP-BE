﻿using Azure;
using BusinessObject.DTO;
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

        // ✅ Tìm User theo UserId
        Task<User> GetUserByIdAsync(Guid userId);

        //Login
        Task<User> LoginAsync(string userName, string password);
        //Logout
        Task<User> GetUserByEmailAsync(string email);

        Task<int> GetTotalUsersAsync(); // ✅ Thêm phương thức này
        Task<List<User>> GetUsersAsync(int skip, int take);
        Task<(Province, District, Ward)> GetLocationIdsAsync(string provinceName, string districtName, string wardName);
        Task<Employee> GetEmployeeByUserIdAsync(Guid userId);
        Task<AgencyAccount> GetAgencyAccountByUserIdAsync(Guid userId);
        Task<Address> GetAddressByIdAsync(int addressId);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> UpdateAgencyAccountAsync(AgencyAccount agencyAccount);
        Task<bool> UpdateAddressAsync(Address address);
        Task<UserRole> GetUserRoleByUserIdAsync(Guid userId);
        Task<bool> UpdateUserRoleAsync(UserRole userRole);
        Task<User> GetUserByUsernameAsync(string username);
        List<UserRole> GetUserRoles(Guid userId);

        Task<List<RegisterAccount>> GetRegisterAccount();


    }

}
