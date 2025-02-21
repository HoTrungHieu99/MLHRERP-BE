using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IAgencyAccountRepository
    {
        Task<List<AgencyAccount>> GetAll();
        Task<AgencyAccount> GetAgencyAccountByIdAsync(long agencyId);
        Task<bool> UpdateAgencyAccountAsync(AgencyAccount agencyAccount);
    }
}
