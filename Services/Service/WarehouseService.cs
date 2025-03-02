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
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepo;
        private readonly ILocationRepository _locationRepository;

        public WarehouseService(IWarehouseRepository warehouseRepo, ILocationRepository locationRepository)
        {
            _warehouseRepo = warehouseRepo;
            _locationRepository = locationRepository;
        }

        public List<Warehouse> GetAllWarehouses() => _warehouseRepo.GetAllWarehouses();

        public Warehouse GetWarehouseByUserId(Guid userId) => _warehouseRepo.GetWarehouseByUserId(userId);

        public void CreateWarehouse(Guid userId, string warehousName, string street, string province, string district, string ward)
        {
            // Kiểm tra xem User đã có Warehouse chưa
            if (_warehouseRepo.GetWarehouseByUserId(userId) != null)
                throw new Exception("Each Employee can only create one Warehouse!");

            // Lấy ID của Province
            var provinceId = _locationRepository.GetProvinceIdByName(province);
            if (provinceId == null) throw new Exception("Province is invalid!");

            // Lấy ID của District
            var districtId = _locationRepository.GetDistrictIdByName(district, provinceId.Value);
            if (districtId == null) throw new Exception("District is invalid!");

            // Lấy ID của Ward
            var wardId = _locationRepository.GetWardIdByName(ward, districtId.Value);
            if (wardId == null) throw new Exception("Ward is invalid!");

            // Tạo Address
            var address = new Address
            {
                Street = street,
                ProvinceId = provinceId.Value,
                DistrictId = districtId.Value,
                WardId = wardId.Value
            };

            _locationRepository.AddAddress(address);

            // Tạo Warehouse
            var warehouse = new Warehouse
            {
                WarehouseName = warehousName,
                UserId = userId,
                AddressId = address.AddressId
            };

            _warehouseRepo.AddWarehouse(warehouse);
        }

        public void UpdateWarehouse(Guid userId, int warehouseId, string warehousName, string street, string province, string district, string ward)
        {
            // Tìm Warehouse của User
            var warehouse = _warehouseRepo.GetWarehouseById(warehouseId);
            if (warehouse == null)
                throw new Exception("Warehouse does not exist!");

            if (warehouse.UserId != userId)
                throw new Exception("You do not have permission to update this Warehouse!");

            // Tìm Address liên kết với Warehouse
            var address = _locationRepository.GetAddressById(warehouse.AddressId);  // ✅ Đảm bảo gọi từ đúng Interface
            if (address == null)
                throw new Exception("Warehouse address does not exist!");

            // Cập nhật Address
            var provinceId = _locationRepository.GetProvinceIdByName(province);
            if (provinceId == null) throw new Exception("Province is invalid!");

            var districtId = _locationRepository.GetDistrictIdByName(district, provinceId.Value);
            if (districtId == null) throw new Exception("Invalid District!");

            var wardId = _locationRepository.GetWardIdByName(ward, districtId.Value);
            if (wardId == null) throw new Exception("Invalid Ward!");

            address.Street = street;
            address.ProvinceId = provinceId.Value;
            address.DistrictId = districtId.Value;
            address.WardId = wardId.Value;

            _locationRepository.UpdateAddress(address);

            // Cập nhật Warehouse
            warehouse.WarehouseName = warehousName;
            _warehouseRepo.UpdateWarehouse(warehouse);
        }


        public async Task<bool> DeleteWarehouseAsync(int warehouseId)
        {
            var warehouse = await _warehouseRepo.GetWarehouseByIdAsync(warehouseId);
            if (warehouse == null)
            {
                return false; // Không tìm thấy Warehouse để xóa
            }

            await _warehouseRepo.DeleteWarehouseAsync(warehouse);
            return true;
        }
    }
}
