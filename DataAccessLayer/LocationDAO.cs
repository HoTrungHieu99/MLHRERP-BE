using BusinessObject.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayer
{
    public class LocationDAO
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string API_BASE_URL = "https://provinces.open-api.vn/api/";

        public static async Task<List<Province>> GetProvincesAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("https://provinces.open-api.vn/api/");
            if (response.IsSuccessStatusCode)
            {
                string jsonData = await response.Content.ReadAsStringAsync();
                return JArray.Parse(jsonData).ToObject<List<Province>>();
            }
            return new List<Province>();
        }

        public static async Task<List<District>> GetDistrictsAsync(int provinceId)
        {
            string url = $"{API_BASE_URL}p/{provinceId}?depth=2";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Lỗi khi gọi API {url}: {response.StatusCode}");
                return new List<District>(); // Trả về danh sách rỗng nếu có lỗi
            }

            string jsonData = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonData) || jsonData == "null")
            {
                Console.WriteLine($"API không có dữ liệu cho provinceId = {provinceId}");
                return new List<District>();
            }

            try
            {
                Console.WriteLine($"Dữ liệu từ API cho provinceId = {provinceId}: {jsonData}");
                // Deserialize JSON thành Province trước, sau đó lấy danh sách Districts
                Province province = JsonConvert.DeserializeObject<Province>(jsonData);
                return province.Districts ?? new List<District>(); // Tránh lỗi nếu danh sách null
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Deserialize JSON: {ex.Message}");
                return new List<District>();
            }
        }



        public static async Task<List<Ward>> GetWardsAsync(int districtId)
        {
            string url = $"{API_BASE_URL}d/{districtId}?depth=2";
            Console.WriteLine($"Đang gọi API: {url}"); // In URL để debug

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Lỗi khi gọi API {url}: {response.StatusCode}");
                return new List<Ward>(); // Trả về danh sách rỗng nếu có lỗi
            }

            string jsonData = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonData) || jsonData == "null")
            {
                Console.WriteLine($"API không có dữ liệu cho districtId = {districtId}");
                return new List<Ward>();
            }

            try
            {
                Console.WriteLine($"Dữ liệu từ API cho districtId = {districtId}: {jsonData}");

                // Deserialize JSON thành District trước, sau đó lấy danh sách Wards
                District district = JsonConvert.DeserializeObject<District>(jsonData);
                return district.Wards ?? new List<Ward>(); // Tránh lỗi nếu danh sách null
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Deserialize JSON: {ex.Message}");
                return new List<Ward>();
            }
        }

    }
}
