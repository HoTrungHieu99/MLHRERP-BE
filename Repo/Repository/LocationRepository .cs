using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly MinhLongDbContext _context;

        public LocationRepository(MinhLongDbContext context)
        {
            _context = context;
        }
        public async Task<List<Province>> GetProvincesAsync()
        {
            return await _context.Provinces.ToListAsync();
        }

        public async Task<List<District>> GetDistrictAsync(int provinceId)
        {
            return await _context.Districts
                .Where(d => d.ProvinceId == provinceId)
                .ToListAsync();
        }

        public async Task<List<Ward>> GetWardsAsync(int districtId)
        {
            return await _context.Wards
                .Where(d => d.DistrictId == districtId)
                .ToListAsync();
        }

        /*public async Task<List<Province>> GetProvincesAsync()
        {
            return await LocationDAO.GetProvincesAsync();
        }

        public async Task<List<District>> GetDistrictAsync(int provinceId)
        {
            return await LocationDAO.GetDistrictsAsync(provinceId);
        }

        public async Task<List<Ward>> GetWardsAsync(int districtId)
        {
            return await LocationDAO.GetWardsAsync(districtId);
        }
        */


        // 🚀 Lấy danh sách tỉnh từ API
        public async Task<List<Province>> GetProvincesFromAPI()
        {
            return await LocationDAO.GetProvincesAsync();
        }

        // 🚀 Lấy danh sách huyện từ API (theo tỉnh)
        public async Task<List<District>> GetDistrictsFromAPI(int provinceId)
        {
            return await LocationDAO.GetDistrictsAsync(provinceId);
        }

        // 🚀 Lấy danh sách xã/phường từ API (theo huyện)
        public async Task<List<Ward>> GetWardsFromAPI(int districtId)
        {
            return await LocationDAO.GetWardsAsync(districtId);
        }

        // 🔥 Lưu danh sách tỉnh vào database
        public async Task SaveProvincesToDatabase(List<Province> provinces)
        {
            var existingProvinces = await _context.Provinces.Select(p => p.ProvinceId).ToListAsync();
            var newProvinces = provinces.Where(p => !existingProvinces.Contains(p.ProvinceId)).ToList();

            if (newProvinces.Any())
            {
                await _context.Provinces.AddRangeAsync(newProvinces);
                await _context.SaveChangesAsync();
            }
        }


        // 🔥 Lưu danh sách huyện vào database
        public async Task SaveDistrictsToDatabase(List<District> districts)
        {
            await _context.Districts.AddRangeAsync(districts);
            await _context.SaveChangesAsync();
        }

        // 🔥 Lưu danh sách xã/phường vào database
        public async Task SaveWardsToDatabase(List<Ward> wards)
        {
            await _context.Wards.AddRangeAsync(wards);
            await _context.SaveChangesAsync();
        }
    }
}
