using BusinessObject.DTO;
using BusinessObject.Models;
using Repo.IRepository;
using Repo.Repository;
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
    public class AgencyAccountService : IAgencyAccountService
    {
        private readonly IAgencyAccountRepository _agencyAccount;
        private readonly IUserRepository _userRepository;
        public AgencyAccountService(IAgencyAccountRepository agencyAccount, IUserRepository userRepository)
        {
            _agencyAccount = agencyAccount;
            _userRepository = userRepository;
        }

        public async Task<List<AgencyAccount>> GetAgencyAccount()
        {
            return await _agencyAccount.GetAll();
        }

        public async Task<AgencyAccount> GetAgencyAccountByUserIdAsync(long agencyId)
        {
            return await _agencyAccount.GetAgencyAccountByIdAsync(agencyId);
        }

        public async Task<bool> UpdateAgencyAccountAsync(long agencyId, AgencyAccountRequest request)
        {
            var agencyAccount = await _agencyAccount.GetAgencyAccountByIdAsync(agencyId);
            if (agencyAccount == null)
                throw new ArgumentException("AgencyAccount does not exist!");

            // ✅ Tìm User theo UserId của Employee
            var user = await _userRepository.GetUserByIdAsync(agencyAccount.UserId);
            if (user == null)
                throw new ArgumentException("User does not exist!");

            // ✅ Validate AgencyName - Viết hoa chữ cái đầu
            if (!string.IsNullOrWhiteSpace(request.AgencyName))
            {
                agencyAccount.AgencyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.AgencyName.ToLower());
            }
            /*// ✅ Validate Address - Viết hoa chữ cái đầu
            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                agencyAccount.Address = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.Address.ToLower());
            }*/
            /*// ✅ Cập nhật LocationId (nếu không phải 0)
            if (request.LocationId != 0)
            {
                agencyAccount.LocationId = request.LocationId;
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
            return await _agencyAccount.UpdateAgencyAccountAsync(agencyAccount);
        }
    }
}
