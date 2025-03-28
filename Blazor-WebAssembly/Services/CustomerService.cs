using System.Net.Http.Json;
using Blazor_WebAssembly.Models;

namespace Blazor_WebAssembly.Services
{
    // 客戶資料服務類別，提供從JSON獲取客戶資料的功能
    public class CustomerService
    {
        private readonly HttpClient _httpClient;
        private List<Customer>? _cachedCustomers;

        // 使用依賴注入取得HttpClient
        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 獲取所有客戶資料
        public async Task<List<Customer>> GetCustomersAsync()
        {
            // 如果已有快取資料，則直接返回
            if (_cachedCustomers != null)
            {
                return _cachedCustomers;
            }

            try
            {
                // 從根目錄的Data文件夾讀取customers.json
                // 在WebAssembly中，我們將JSON文件放在wwwroot/sample-data目錄下
                var data = await _httpClient.GetFromJsonAsync<CustomerData>("sample-data/customers.json");
                _cachedCustomers = data?.Customers ?? new List<Customer>();
                return _cachedCustomers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"獲取客戶資料時出錯: {ex.Message}");
                return new List<Customer>();
            }
        }

        // 根據ID獲取客戶
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            var customers = await GetCustomersAsync();
            return customers.FirstOrDefault(c => c.CustomerID == id);
        }
    }
}