using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text;
using System.Net.Http;

namespace Blazor_Server.Data
{
    // 客戶資料服務類別，提供從JSON讀取客戶資料的功能，並新增編輯、新增、刪除的功能
    public class CustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpClientFactory? _httpClientFactory;
        private static List<Customer>? _allCustomers;

        // JSON檔案路徑
        private string JsonFilePath => Path.Combine(
            Directory.GetParent(_environment.ContentRootPath)?.FullName ?? "",
            "Data",
            "customers.json");

        // 使用依賴注入取得必要服務
        public CustomerService(
            ILogger<CustomerService> logger, 
            IWebHostEnvironment environment,
            IHttpClientFactory? httpClientFactory = null)
        {
            _logger = logger;
            _environment = environment;
            _httpClientFactory = httpClientFactory;
        }

        // 獲取所有客戶資料
        public async Task<List<Customer>> GetCustomersAsync()
        {
            // 如果快取中沒有資料，則從JSON檔案讀取
            if (_allCustomers == null)
            {
                await LoadCustomersFromJsonAsync();
            }
            return _allCustomers ?? new List<Customer>();
        }

        // 內部類別：用於JSON反序列化
        internal class CustomerData
        {
            public List<Customer> Customers { get; set; } = new();
        }

        // 從JSON檔案讀取客戶資料
        private async Task LoadCustomersFromJsonAsync()
        {
            try
            {
                // 嘗試讀取 JSON 檔案的絕對路徑
                string jsonFilePath = JsonFilePath;

                _logger.LogInformation($"嘗試讀取 JSON 檔案: {jsonFilePath}");

                if (File.Exists(jsonFilePath))
                {
                    _logger.LogInformation("找到 JSON 檔案，開始讀取...");
                    // 讀取檔案內容
                    string jsonString = await File.ReadAllTextAsync(jsonFilePath, Encoding.UTF8);
                    _logger.LogInformation($"JSON 內容長度: {jsonString.Length} 字符");

                    // 設定 JSON 反序列化選項
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // 屬性名稱不區分大小寫
                    };

                    // 將 JSON 字串轉換為 C# 物件
                    var data = JsonSerializer.Deserialize<CustomerData>(jsonString, options);
                    _allCustomers = data?.Customers ?? new List<Customer>();
                    _logger.LogInformation($"成功讀取到 {_allCustomers.Count} 個客戶資料");
                }
                else
                {
                    _logger.LogWarning($"JSON 檔案不存在: {jsonFilePath}，嘗試使用 HTTP 請求獲取...");
                    
                    // 如果檔案不存在，嘗試通過 HTTP 請求讀取
                    try
                    {
                        string jsonString;
                        
                        if (_httpClientFactory != null)
                        {
                            var client = _httpClientFactory.CreateClient();
                            client.BaseAddress = new Uri("http://localhost:5000");
                            jsonString = await client.GetStringAsync("/data/customers.json");
                        }
                        else
                        {
                            using var client = new HttpClient();
                            client.BaseAddress = new Uri("http://localhost:5000");
                            jsonString = await client.GetStringAsync("/data/customers.json");
                        }
                        
                        _logger.LogInformation($"透過 HTTP 成功獲取 JSON 資料，長度: {jsonString.Length} 字符");
                        
                        // 反序列化 JSON 字串
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        
                        var data = JsonSerializer.Deserialize<CustomerData>(jsonString, options);
                        _allCustomers = data?.Customers ?? new List<Customer>();
                        _logger.LogInformation($"成功讀取到 {_allCustomers.Count} 個客戶資料");
                    }
                    catch (Exception httpEx)
                    {
                        _logger.LogError(httpEx, "HTTP 請求獲取 JSON 資料失敗");
                        throw; // 重新拋出異常，交由外層處理
                    }
                }
            }
            catch (Exception ex)
            {
                // 錯誤處理
                _logger.LogError(ex, "讀取客戶資料時發生錯誤");
                _allCustomers = new List<Customer>();
                
                // 發生錯誤時提供基本的測試資料，確保應用不會完全無法使用
                _allCustomers.Add(new Customer { 
                    CustomerID = 999, 
                    CustomerName = "錯誤回退用戶", 
                    CustomerLocation = "系統", 
                    Email = "error@example.com", 
                    Phone = "000-000-000",
                    Address = "讀取資料失敗，請檢查日誌"
                });
            }
        }

        // 取得指定ID的客戶
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
                _allCustomers = customers;
                
                // 保存到JSON文件
                await SaveCustomersToJsonAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加客戶時發生錯誤");
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
                    _logger.LogWarning($"未找到ID為 {customer.CustomerID} 的客戶");
                    return false;
                }
                
                // 更新客戶資料
                existingCustomer.CustomerName = customer.CustomerName;
                existingCustomer.CustomerLocation = customer.CustomerLocation;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Address = customer.Address;
                
                // 保存到JSON文件
                await SaveCustomersToJsonAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新ID為 {customer.CustomerID} 的客戶時發生錯誤");
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
                    _logger.LogWarning($"未找到ID為 {id} 的客戶");
                    return false;
                }
                
                customers.Remove(customer);
                _allCustomers = customers;
                
                // 保存到JSON文件
                await SaveCustomersToJsonAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"刪除ID為 {id} 的客戶時發生錯誤");
                return false;
            }
        }
        
        // 保存客戶資料到JSON文件
        private async Task SaveCustomersToJsonAsync()
        {
            try
            {
                if (_allCustomers == null)
                {
                    _logger.LogWarning("嘗試保存空的客戶列表");
                    return;
                }
                
                // 準備序列化選項
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                // 將客戶數據包裝進容器類
                var data = new CustomerData { Customers = _allCustomers };
                
                // 序列化為JSON
                string jsonString = JsonSerializer.Serialize(data, options);
                
                // 保存到文件
                await File.WriteAllTextAsync(JsonFilePath, jsonString, Encoding.UTF8);
                
                _logger.LogInformation($"客戶資料已保存到 {JsonFilePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存客戶資料時發生錯誤");
                throw;
            }
        }
    }
}