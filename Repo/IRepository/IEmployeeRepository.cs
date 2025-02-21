using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAll();
        Task<Employee> GetEmployeeByUserIdAsync(long employeeId);
        Task<bool> UpdateEmployeeAsync(Employee employee);
    }

}
