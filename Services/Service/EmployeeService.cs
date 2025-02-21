using BusinessObject.DTO;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
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

        // Cập nhật thông tin Employee
        public async Task<bool> UpdateEmployeeAsync(long employeeId, UpdateEmployeeRequest request)
        {
            var employee = await _employeeRepository.GetEmployeeByUserIdAsync(employeeId);
            if (employee == null)
                throw new ArgumentException("Employee does not exist!");

            // Cập nhật thông tin, giữ nguyên giá trị cũ nếu trường bị bỏ trống hoặc null
            employee.FullName = !string.IsNullOrWhiteSpace(request.FullName) ? request.FullName : employee.FullName;
            employee.Position = !string.IsNullOrWhiteSpace(request.Position) ? request.Position : employee.Position;
            employee.Department = !string.IsNullOrWhiteSpace(request.Department) ? request.Department : employee.Department;
            employee.LocationId = request.LocationId != 0 ? request.LocationId : employee.LocationId;

            return await _employeeRepository.UpdateEmployeeAsync(employee);
        }

    }

}
