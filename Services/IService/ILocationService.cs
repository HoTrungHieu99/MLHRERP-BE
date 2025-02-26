using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface ILocationService
    {
        Task<List<Province>> GetProvincesAsync();
        Task<List<District>> GetDistrictAsync(int provinceId);
        Task<List<Ward>> GetWardsAsync(int districtId);



        Task ImportAllLocations();
    }
}
