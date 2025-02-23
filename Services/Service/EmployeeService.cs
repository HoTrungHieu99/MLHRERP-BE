using BusinessObject.DTO;
using BusinessObject.Models;
using Repo.IRepository;
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
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IUserRepository userRepository)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Employee>> GetEmployee()
        {
            return await _employeeRepository.GetAll();
        }

        // Lấy thông tin Employee
        public async Task<Employee> GetEmployeeByUserIdAsync(long employeeId)
        {
            return await _employeeRepository.GetEmployeeByUserIdAsync(employeeId);
        }

        public async Task<bool> UpdateEmployeeAsync(long employeeId, UpdateEmployeeRequest request)
        {
            // ✅ Tìm Employee theo UserId
            var employee = await _employeeRepository.GetEmployeeByUserIdAsync(employeeId);
            if (employee == null)
                throw new ArgumentException("Employee does not exist!");

            // ✅ Tìm User theo UserId của Employee
            var user = await _userRepository.GetUserByIdAsync(employee.UserId);
            if (user == null)
                throw new ArgumentException("User does not exist!");

            // ✅ Validate FullName - Viết hoa chữ cái đầu
            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                employee.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.FullName.ToLower());
            }

            /*// ✅ Cập nhật LocationId (nếu không phải 0)
            if (request.LocationId != 0)
            {
                employee.LocationId = request.LocationId;
            }*/

            
            // ✅ Nếu có mật khẩu mới, hash trước khi lưu vào User
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
                user.Password = request.Password;
            }


            // ✅ Cập nhật User trước, sau đó Employee
            await _userRepository.UpdateUserAsync(user);
            return await _employeeRepository.UpdateEmployeeAsync(employee);
        }


    }

}
