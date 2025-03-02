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
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _provinceRepository;

        public LocationService(ILocationRepository provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        public async Task<List<Province>> GetProvincesAsync()
        {
            return await _provinceRepository.GetProvincesAsync();
        }

        public async Task<List<District>> GetDistrictAsync(int provinceId)
        {
            return await _provinceRepository.GetDistrictAsync(provinceId);
        }

        public async Task<List<Ward>> GetWardsAsync(int districtId)
        {
            return await _provinceRepository.GetWardsAsync(districtId);
        }

        public async Task ImportAllLocations()
        {
            try
            {
                // 1️⃣ Nhập danh sách tỉnh từ API
                var provinces = await _provinceRepository.GetProvincesFromAPI();
                if (provinces != null && provinces.Any())
                {
                    await _provinceRepository.SaveProvincesToDatabase(provinces);
                }

                // 2️⃣ Nhập danh sách huyện từ API
                List<District> allDistricts = new List<District>();
                foreach (var province in provinces)
                {
                    var districts = await _provinceRepository.GetDistrictsFromAPI(province.ProvinceId);
                    if (districts != null && districts.Any())
                    {
                        allDistricts.AddRange(districts);
                    }
                }
                await _provinceRepository.SaveDistrictsToDatabase(allDistricts);

                // 3️⃣ Nhập danh sách xã từ API
                List<Ward> allWards = new List<Ward>();
                foreach (var district in allDistricts)
                {
                    var wards = await _provinceRepository.GetWardsFromAPI(district.DistrictId);
                    if (wards != null && wards.Any())
                    {
                        allWards.AddRange(wards);
                    }
                }
                await _provinceRepository.SaveWardsToDatabase(allWards);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LỖI: {ex.Message}");
                Console.WriteLine($"INNER EXCEPTION: {ex.InnerException?.Message}");
                throw new Exception("Lỗi nhập dữ liệu: " + ex.Message, ex);
            }
        }
    }
}
