using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MinhLongDbContext _context;

        public UserRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        // ✅ Lưu yêu cầu đăng ký vào RegisterAccount
        public async Task<RegisterAccount> RegisterUserRequestAsync(RegisterAccount registerAccount)
        {
            try
            {
                // ✅ Tạo mật khẩu ngẫu nhiên 9 ký tự
                string rawPassword = PasswordHelper.GenerateRandomPassword();

                registerAccount.Password = BCrypt.Net.BCrypt.HashPassword(rawPassword);
                // ✅ Kiểm tra nếu Username, Email hoặc Phone đã tồn tại
                var existingAccount = await _context.RegisterAccounts
                    .FirstOrDefaultAsync(u => u.Username == registerAccount.Username ||
                                              u.Email == registerAccount.Email ||
                                              u.Phone == registerAccount.Phone);
                if (existingAccount != null)
                {
                    throw new ArgumentException("Username, Email, or Phone already exists!");
                }

              
                // ✅ Nếu Password rỗng hoặc null, đặt giá trị mặc định là "1"
                if (string.IsNullOrWhiteSpace(registerAccount.Password))
                {
                    registerAccount.Password = rawPassword;
                }
                // ✅ Lưu tài khoản đăng ký vào RegisterAccount
                _context.RegisterAccounts.Add(registerAccount);
                await _context.SaveChangesAsync();

                return registerAccount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Error saving registration account: " + ex.Message);
            }
        }

        // ✅ Admin duyệt tài khoản và lưu vào bảng User + Employee hoặc Agency
        public async Task<bool> ApproveUserAsync(int registerId)
        {
            try
            {
                var registerAccount = await _context.RegisterAccounts.FindAsync(registerId);
                if (registerAccount == null || registerAccount.IsApproved)
                    return false; // Tài khoản không tồn tại hoặc đã được duyệt trước đó

                // ✅ Tạo Address dựa trên thông tin từ RegisterAccount
                (Province province, District district, Ward ward) = await GetLocationIdsAsync(
                    registerAccount.ProvinceName, registerAccount.DistrictName, registerAccount.WardName
                );

                var newAddress = new Address
                {
                    Street = registerAccount.Street,
                    WardId = ward.WardId,
                    DistrictId = district.DistrictId,
                    ProvinceId = province.ProvinceId
                };

                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync(); // Lưu để lấy AddressId

                // ✅ Tạo User
                var user = new User
                {
                    Username = registerAccount.Username,
                    Email = registerAccount.Email,
                    Phone = registerAccount.Phone,
                    UserType = registerAccount.UserType,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerAccount.Password), // Có thể hash trước khi lưu
                    Status = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // Lưu trước để có UserId

                // ✅ Nếu UserType là EMPLOYEE, lưu vào bảng Employee
                if (registerAccount.UserType.Equals("EMPLOYEE", StringComparison.OrdinalIgnoreCase))
                {
                    var employee = new Employee
                    {
                        UserId = user.UserId,
                        FullName = registerAccount.FullName,
                        Position = registerAccount.Position,
                        Department = registerAccount.Department,
                        AddressId = newAddress.AddressId // Sử dụng AddressId thay vì LocationId
                    };

                    _context.Employees.Add(employee);
                }
                // ✅ Nếu UserType là AGENCY, lưu vào bảng AgencyAccount
                else if (registerAccount.UserType.Equals("AGENCY", StringComparison.OrdinalIgnoreCase))
                {
                    var agency = new AgencyAccount
                    {
                        UserId = user.UserId,
                        AgencyName = registerAccount.AgencyName,
                        AddressId = newAddress.AddressId
                    };

                    _context.AgencyAccounts.Add(agency);
                }

                // ✅ Đánh dấu tài khoản đã được duyệt
                registerAccount.IsApproved = true;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Error while approving user: " + ex.Message);
            }
        }

        // ✅ Phương thức lấy Province, District, Ward dựa trên tên
        private async Task<(Province, District, Ward)> GetLocationIdsAsync(string provinceName, string districtName, string wardName)
        {
            var province = await _context.Provinces.FirstOrDefaultAsync(p => p.ProvinceName == provinceName);
            if (province == null) throw new Exception($"Province '{provinceName}' not found.");

            var district = await _context.Districts.FirstOrDefaultAsync(d => d.DistrictName == districtName && d.ProvinceId == province.ProvinceId);
            if (district == null) throw new Exception($"District '{districtName}' not found in Province '{provinceName}'.");

            var ward = await _context.Wards.FirstOrDefaultAsync(w => w.WardName == wardName && w.DistrictId == district.DistrictId);
            if (ward == null) throw new Exception($"Ward '{wardName}' not found in District '{districtName}'.");

            return (province, district, ward);
        }


        // ✅ Cập nhật User (bao gồm mật khẩu)
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
        // ✅ Tìm User theo UserId

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        // Lấy thông tin RegisterAccount theo ID
        public async Task<RegisterAccount> GetRegisterAccountByIdAsync(int registerId)
        {
            return await _context.RegisterAccounts.FindAsync(registerId);
        }
        //Login
        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new ArgumentException("Invalid email or password.");
            }

            // ✅ Nếu mật khẩu đã hash bằng BCrypt, kiểm tra bằng BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new ArgumentException("Invalid email or password.");
            }

            return user;
        }

        //Logout
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

    }


    public static class PasswordHelper
    {
        public static string GenerateRandomPassword()
        {
            const int length = 9; // ✅ Cố định độ dài là 9 ký tự
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            Random random = new Random();

            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
