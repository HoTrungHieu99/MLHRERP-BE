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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly MinhLongDbContext _context;

        public EmployeeRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employees.ToListAsync();
        }


        // Lấy thông tin Employee theo UserId
        public async Task<Employee> GetEmployeeByUserIdAsync(long employeeId)
        {
            return  _context.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
        }

        // ✅ Cập nhật Employee
        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            return await _context.SaveChangesAsync() > 0;
        }
    }

}
