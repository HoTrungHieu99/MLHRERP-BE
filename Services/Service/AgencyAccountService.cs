using BusinessObject.DTO;
using BusinessObject.Models;
using Repo.IRepository;
using Repo.Repository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class AgencyAccountService : IAgencyAccountService
    {
        private readonly IAgencyAccountRepository _agencyAccount;

        public AgencyAccountService(IAgencyAccountRepository agencyAccount)
        {
            _agencyAccount = agencyAccount;
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

            // Cập nhật thông tin, giữ nguyên giá trị cũ nếu trường bị bỏ trống hoặc null
            agencyAccount.AgencyName = !string.IsNullOrWhiteSpace(request.AgencyName) ? request.AgencyName : agencyAccount.AgencyName;
            agencyAccount.Address = !string.IsNullOrWhiteSpace(request.Address) ? request.Address : agencyAccount.Address;

            return await _agencyAccount.UpdateAgencyAccountAsync(agencyAccount);
        }
    }
}
