using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IUserService
    {
        Task<User> Login(string username, string password);
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(long id);
        Task AddUser(User user);
        Task<User> UpdateUser(User user);
        Task DeleteUser(long id);
    }
}
