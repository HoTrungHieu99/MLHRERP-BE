using BusinessObject.Models;
using DataAccessLayer;
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
        private readonly UserDAO _userDAO;

        public UserRepository()
        {
            _userDAO = UserDAO.Instance;
        }

        public async Task AddUser(User user)
        {
            // Kiểm tra nếu UserName đã tồn tại
            var existingUsers = await _userDAO.GetAllUsers();
            if (existingUsers.Any(u => u.Username == user.Username))
            {
                throw new InvalidOperationException("UserName already exists in the system!");
            }

            // Kiểm tra nếu Email đã tồn tại
            if (existingUsers.Any(u => u.Email == user.Email))
            {
                throw new InvalidOperationException("Email already exists in the system!");
            }

            // Kiểm tra nếu User_Type hợp lệ
            var validUserTypes = new List<string> { "EMPLOYEE", "AGENT" };
            if (!validUserTypes.Contains(user.UserType))
            {
                throw new ArgumentException("Invalid User_Type. Only accepted: EMPLOYEE or AGENT!");
            }

            await _userDAO.AddUser(user);
        }

        public async Task DeleteUser(long id)
        {
            await _userDAO.DeleteUser(id);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userDAO.GetAllUsers();
        }

        public async Task<User> GetUserById(long id)
        {
            return await _userDAO.GetUserById(id);
        }

        public async Task<User> Login(string username, string password)
        {
            return await _userDAO.Login(username, password);
        }

        public async Task<User> UpdateUser(User user)
        {
            var existingUser = await _userDAO.GetUserById(user.UserId);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"UserID {user.UserId} does not exist!");
            }

            // Kiểm tra nếu UserName đã tồn tại ở User khác
            var existingUsers = await _userDAO.GetAllUsers();
            if (existingUsers.Any(u => u.Username == user.Username && u.UserId != user.UserId))
            {
                throw new InvalidOperationException("UserName already exists in the system!");
            }

            // Kiểm tra nếu Email đã tồn tại ở User khác
            if (existingUsers.Any(u => u.Email == user.Email && u.UserId != user.UserId))
            {
                throw new InvalidOperationException("Email already exists in the system!");
            }

            // Kiểm tra nếu Password không được giống Password cũ
            if (user.Password == existingUser.Password)
            {
                throw new InvalidOperationException("New Password must not be the same as old Password!");
            }

            return await _userDAO.UpdateUser(user);
        }
    }
}
