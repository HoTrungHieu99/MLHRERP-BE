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

        public async Task<bool> AddToEmployeeOrAgencyAsync(User user)
        {
            if (user.UserType.Equals("EMPLOYEE", StringComparison.OrdinalIgnoreCase))
            {
                var employee = new Employee
                {
                    FullName = user.Username,
                    Position = "Unknown",
                    Department = "Unknown",
                    UserId = user.UserId,
                    LocationId = 1  // Giá trị mặc định hoặc lấy từ request nếu có
                };

                _context.Employees.Add(employee);
            }
            else if (user.UserType.Equals("AGENCY", StringComparison.OrdinalIgnoreCase))
            {
                var agency = new AgencyAccount
                {
                    AgencyName = user.Username + " Agency",
                    Address = "Unknown",
                    UserId = user.UserId,
                    LocationId = 1 // Giá trị mặc định
                };

                _context.AgencyAccounts.Add(agency);
            }
            else
            {
                throw new ArgumentException("UserType không hợp lệ");
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Kiểm tra Email đã tồn tại chưa
        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Kiểm tra Số điện thoại đã tồn tại chưa
        public async Task<bool> IsPhoneExistAsync(string phone)
        {
            return await _context.Users.AnyAsync(u => u.Phone == phone);
        }

        // Kiểm tra Username đã tồn tại chưa
        public async Task<bool> IsUsernameExistAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        // Kiểm tra FullName đã tồn tại chưa (Employee)
        public async Task<bool> IsFullNameExistAsync(string fullName)
        {
            return await _context.Employees.AnyAsync(e => e.FullName == fullName);
        }

        /*// Kiểm tra Position đã tồn tại chưa
        public async Task<bool> IsPositionExistAsync(string position)
        {
            return await _context.Employees.AnyAsync(e => e.Position == position);
        }*/

        // Kiểm tra Department đã tồn tại chưa
        /*public async Task<bool> IsDepartmentExistAsync(string department)
        {
            return await _context.Employees.AnyAsync(e => e.Department == department);
        }*/

        // Kiểm tra LocationId đã tồn tại chưa
        public async Task<bool> IsLocationExistAsync(int locationId)
        {
            return await _context.Locations.AnyAsync(l => l.LocationId == locationId);
        }

        // Kiểm tra AgencyName đã tồn tại chưa (Agent)
        public async Task<bool> IsAgencyNameExistAsync(string agencyName)
        {
            return await _context.AgencyAccounts.AnyAsync(a => a.AgencyName == agencyName);
        }

        // Kiểm tra Address đã tồn tại chưa (Agent)
        /*public async Task<bool> IsAddressExistAsync(string address)
        {
            return await _context.AgencyAccounts.AnyAsync(a => a.Address == address);
        }*/

        // Đăng ký User (bao gồm kiểm tra thông tin trùng lặp)
        public async Task<User> RegisterUserAsync(User user)
        {
            if (await IsUsernameExistAsync(user.Username))
                throw new ArgumentException("Username already exists!");

            if (await IsEmailExistAsync(user.Email))
                throw new ArgumentException("Email already exists!");

            if (await IsPhoneExistAsync(user.Phone))
                throw new ArgumentException("Phone number already exists!");

            // Hash password
            //user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Đặt trạng thái mặc định là 0 (false)
            user.Status = false;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UpdateUserStatusAsync(long userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User does not exist!");

            // Cập nhật trạng thái thành 1 (true)
            user.Status = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserByIdAsync(long userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }


}
