using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface ILocationRepository
    {
        Task<List<Province>> GetProvincesAsync();
        Task<List<District>> GetDistrictAsync(int provinceId);
        Task<List<Ward>> GetWardsAsync(int districtId);

        int? GetProvinceIdByName(string provinceName);
        int? GetDistrictIdByName(string districtName, int provinceId);
        int? GetWardIdByName(string wardName, int districtId);
        void AddAddress(Address address);
        void UpdateAddress(Address address);

        Address GetAddressById(int addressId);

        //Code Nhap Du lieu tinh huyen xa vao db
        Task<List<Province>> GetProvincesFromAPI();
        Task<List<District>> GetDistrictsFromAPI(int provinceId);
        Task<List<Ward>> GetWardsFromAPI(int districtId);

        Task SaveProvincesToDatabase(List<Province> provinces);
        Task SaveDistrictsToDatabase(List<District> districts);
        Task SaveWardsToDatabase(List<Ward> wards);
    }
}
