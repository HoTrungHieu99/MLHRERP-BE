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

        // Lưu yêu cầu đăng ký vào RegisterAccount
        public async Task<RegisterAccount> RegisterUserRequestAsync(RegisterAccount registerAccount)
        {
            // ✅ Tạo mật khẩu ngẫu nhiên 9 ký tự
            string rawPassword = PasswordHelper.GenerateRandomPassword();

            // ✅ Hash mật khẩu trước khi lưu vào DB
            //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);
            try
            {
                // Kiểm tra nếu Email hoặc Phone đã tồn tại
                var existingAccount = await _context.RegisterAccounts
                    .FirstOrDefaultAsync(u => u.Username == registerAccount.Username || u.Email == registerAccount.Email 
                    || u.Phone == registerAccount.Phone || u.FullName == registerAccount.FullName || u.AgencyName == registerAccount.AgencyName);
                if (existingAccount != null)
                {
                    throw new ArgumentException("Email or phone number or full name or user name or agency name already exists!");
                }

                // ✅ Nếu Password rỗng hoặc null, đặt giá trị mặc định là "1"
                if (string.IsNullOrWhiteSpace(registerAccount.Password))
                {
                    registerAccount.Password = rawPassword;
                }

                // ✅ Hash mật khẩu trước khi lưu vào database
                //registerAccount.Password = BCrypt.Net.BCrypt.HashPassword(registerAccount.Password);

                // ✅ Lưu vào database
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


        // Lấy thông tin RegisterAccount theo ID
        public async Task<RegisterAccount> GetRegisterAccountByIdAsync(int registerId)
        {
            return await _context.RegisterAccounts.FindAsync(registerId);
        }


        // Duyệt tài khoản và lưu vào bảng User + Employee hoặc Agency
        public async Task<bool> ApproveUserAsync(int registerId)
        {
            try
            {
                var registerAccount = await _context.RegisterAccounts.FindAsync(registerId);
                if (registerAccount == null || registerAccount.IsApproved)
                    return false; // Tài khoản không tồn tại hoặc đã duyệt

                

                // ✅ Tạo User trước
                var user = new User
                {
                    Username = registerAccount.Username,
                    Email = registerAccount.Email,
                    Phone = registerAccount.Phone,
                    UserType = registerAccount.UserType,
                    Password = registerAccount.Password, // ✅ Lưu mật khẩu đã hash
                    Status = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // ✅ Lưu trước để có UserId

                // ✅ Nếu là Employee, lưu vào bảng Employee
                if (registerAccount.UserType.Equals("EMPLOYEE", StringComparison.OrdinalIgnoreCase))
                {
                    var employee = new Employee
                    {
                        UserId = user.UserId, // ✅ UserId đã có sau khi lưu User
                        FullName = registerAccount.FullName,
                        Position = registerAccount.Position,
                        Department = registerAccount.Department,
                        LocationId = registerAccount.LocationId ?? 1 // ✅ Đặt mặc định nếu null
                    };

                    _context.Employees.Add(employee);
                }
                // ✅ Nếu là Agency, lưu vào bảng AgencyAccount
                else if (registerAccount.UserType.Equals("AGENCY", StringComparison.OrdinalIgnoreCase))
                {
                    var agency = new AgencyAccount
                    {
                        UserId = user.UserId, // ✅ UserId đã có sau khi lưu User
                        AgencyName = registerAccount.AgencyName,
                        Address = registerAccount.Address ?? "N/A", // ✅ Đảm bảo không null
                        LocationId = registerAccount.LocationId ?? 1
                    };

                    _context.AgencyAccounts.Add(agency);
                }

                // ✅ Đánh dấu tài khoản đã duyệt
                registerAccount.IsApproved = true;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database Error: {dbEx.InnerException?.Message}");
                throw new Exception("Database error: " + dbEx.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Error while approving user: " + ex.Message);
            }
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
