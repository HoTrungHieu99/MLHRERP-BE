using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class UserDAO
    {
        private readonly MinhLongDbContext _dbContext;
        private static UserDAO _instance = null;

        private UserDAO()
        {
            _dbContext = new MinhLongDbContext();
        }

        public static UserDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserDAO();
                }
                return _instance;
            }
        }

        public async Task<User> Login(string email, string password)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserById(long id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task AddUser(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> UpdateUser(User user)
        {
            var existingUser = await _dbContext.Users.FindAsync(user.UserId);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"UserID {user.UserId} does not exist!");
            }

            existingUser.Username = user.Username ?? existingUser.Username;
            existingUser.Email = user.Email ?? existingUser.Email;
            existingUser.Password = user.Password ?? existingUser.Password;
            existingUser.UserType = user.UserType ?? existingUser.UserType;
            existingUser.Status = user.Status;

            await _dbContext.SaveChangesAsync();
            return existingUser;
        }

        public async Task DeleteUser(long id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"No User with ID {id} found to delete!");
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
