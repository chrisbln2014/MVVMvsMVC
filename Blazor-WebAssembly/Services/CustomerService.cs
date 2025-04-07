using System.Net.Http.Json;
using Blazor_WebAssembly.Models;
using System.Text.Json;

namespace Blazor_WebAssembly.Services
{
    // 客戶資料服務類別，提供從JSON獲取客戶資料的功能，並新增編輯、新增、刪除的功能
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
                // 從 MainWeb 應用提供的數據 API 獲取客戶資料
                // 注意：這裡使用相對路徑，因為 Blazor WebAssembly 已經被託管在 MainWeb 項目中
                var data = await _httpClient.GetFromJsonAsync<CustomerData>("/data/customers.json");
                
                if (data?.Customers != null)
                {
                    Console.WriteLine($"成功獲取到 {data.Customers.Count} 筆客戶資料");
                    _cachedCustomers = data.Customers;
                }
                else
                {
                    Console.WriteLine("獲取到的客戶資料為空");
                    _cachedCustomers = new List<Customer>();
                }
                
                return _cachedCustomers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"獲取客戶資料時出錯: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"內部錯誤: {ex.InnerException.Message}");
                }
                return new List<Customer>();
            }
        }

        // 根據ID獲取客戶
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            var customers = await GetCustomersAsync();
            return customers.FirstOrDefault(c => c.CustomerID == id);
        }
        
        // 添加新客戶
        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            try
            {
                var customers = await GetCustomersAsync();
                
                // 如果集合為空，設置ID為1，否則設置為最大ID+1
                if (customers.Count == 0)
                {
                    customer.CustomerID = 1;
                }
                else
                {
                    customer.CustomerID = customers.Max(c => c.CustomerID) + 1;
                }
                
                customers.Add(customer);
                _cachedCustomers = customers;
                
                // 在實際應用中，這裡應該是API調用保存數據
                // 在Blazor WebAssembly中，由於客户端無法直接操作服務器文件系統，
                // 所以這裡只是更新了緩存，實際應用中會通過API保存
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加客戶時出錯: {ex.Message}");
                return false;
            }
        }
        
        // 更新客戶資料
        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var customers = await GetCustomersAsync();
                var existingCustomer = customers.FirstOrDefault(c => c.CustomerID == customer.CustomerID);
                
                if (existingCustomer == null)
                {
                    return false;
                }
                
                // 更新客戶資料
                existingCustomer.CustomerName = customer.CustomerName;
                existingCustomer.CustomerLocation = customer.CustomerLocation;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Address = customer.Address;
                
                _cachedCustomers = customers;
                
                // 在實際應用中，這裡應該是API調用保存數據
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新客戶時出錯: {ex.Message}");
                return false;
            }
        }
        
        // 刪除客戶
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                var customers = await GetCustomersAsync();
                var customer = customers.FirstOrDefault(c => c.CustomerID == id);
                
                if (customer == null)
                {
                    return false;
                }
                
                customers.Remove(customer);
                _cachedCustomers = customers;
                
                // 在實際應用中，這裡應該是API調用保存數據
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"刪除客戶時出錯: {ex.Message}");
                return false;
            }
        }
    }
}