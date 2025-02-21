using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IUserRepository
    {
        Task<User> RegisterUserAsync(User user);
        Task<bool> UpdateUserStatusAsync(long userId);
        Task<bool> AddToEmployeeOrAgencyAsync(User user);
        Task<bool> IsEmailExistAsync(string email);
        Task<bool> IsPhoneExistAsync(string phone);
        Task<bool> IsUsernameExistAsync(string username); 
        Task<bool> IsFullNameExistAsync(string fullName);
        //Task<bool> IsPositionExistAsync(string position);
        //<bool> IsDepartmentExistAsync(string department);
        Task<bool> IsLocationExistAsync(int locationId);
        Task<bool> IsAgencyNameExistAsync(string agencyName);
        //Task<bool> IsAddressExistAsync(string address);
        Task<User> GetUserByIdAsync(long userId);
    }

}
