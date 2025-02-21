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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public Task AddUser(User user)
        {
            return _userRepository.AddUser(user);
        }

        public Task DeleteUser(long id)
        {
            return _userRepository.DeleteUser(id);
        }

        public Task<List<User>> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public Task<User> GetUserById(long id)
        {
            return _userRepository.GetUserById(id);
        }

        public Task<User> Login(string username, string password)
        {
            var user = _userRepository.Login(username, password);
            return user;
        }

        public Task<User> UpdateUser(User user)
        {
            return _userRepository.UpdateUser(user);
        }
    }
}
