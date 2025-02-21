using BusinessObject.DTO;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IAgencyAccountService
    {
        Task<List<AgencyAccount>> GetAgencyAccount();
        Task<AgencyAccount> GetAgencyAccountByUserIdAsync(long agencyId);
        Task<bool> UpdateAgencyAccountAsync(long agencyId, AgencyAccountRequest request);
    }
}
