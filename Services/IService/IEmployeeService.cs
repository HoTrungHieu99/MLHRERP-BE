using BusinessObject.DTO;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployee();
        Task<Employee> GetEmployeeByUserIdAsync(long agencyId);
        Task<bool> UpdateEmployeeAsync(long employeeId, UpdateEmployeeRequest request);
    }

}
