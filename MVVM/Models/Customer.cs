using System.Text.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.IO;

namespace MVVM.Models
{
    // 客戶資料模型類別，代表單個客戶的資料結構
    public class Customer
    {
        // required 關鍵字表示這些屬性在創建物件時必須要有值
        public required int CustomerID { get; set; }           // 客戶編號，唯一識別碼
        public required string CustomerName { get; set; }      // 客戶名稱
        public required string CustomerLocation { get; set; }  // 客戶所在地
        public string? Email { get; set; }                    // 電子郵件（可為空）
        public string? Phone { get; set; }                    // 電話（可為空）
        public string? Address { get; set; }                  // 地址（可為空）

        // 內部類別：用於 JSON 反序列化
        internal class CustomerData
        {
            public List<Customer> Customers { get; set; } = new();
        }
        
        // 靜態快取，用於儲存所有客戶資料
        private static List<Customer>? _allCustomers;
        
        // 使用相對路徑定義JSON檔案路徑
        private static readonly string jsonFilePath = Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName ?? "",
            "Data", 
            "customers.json");

        // 靜態方法：獲取所有客戶資料
        public static List<Customer> GetAllCustomers()
        {
            // 如果快取中沒有資料，則從 JSON 檔案讀取
            if (_allCustomers == null)
            {
                LoadCustomersFromJson();
            }
            return _allCustomers ?? new List<Customer>();
        }

        // 靜態方法：從JSON讀取客戶資料
        private static void LoadCustomersFromJson()
        {
            try
            {
                Console.WriteLine($"JSON 文件路徑: {jsonFilePath}");

                // 檢查檔案是否存在
                if (File.Exists(jsonFilePath))
                {
                    Console.WriteLine("找到 JSON 文件，開始讀取...");
                    // 使用 UTF8 編碼讀取檔案內容
                    string jsonString = File.ReadAllText(jsonFilePath, Encoding.UTF8);
                    Console.WriteLine($"JSON 內容: {jsonString}");

                    // 設定 JSON 反序列化選項
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // 設定屬性名稱不區分大小寫
                    };

                    // 將 JSON 字串轉換為 C# 物件
                    var data = JsonSerializer.Deserialize<CustomerData>(jsonString, options);
                    _allCustomers = data?.Customers ?? new List<Customer>();
                    Console.WriteLine($"成功讀取到 {_allCustomers.Count} 個客戶資料");
                }
                else
                {
                    _allCustomers = new List<Customer>();
                    Console.WriteLine($"檔案不存在: {jsonFilePath}");
                }
            }
            catch (Exception ex)
            {
                // 錯誤處理：記錄錯誤訊息並返回空列表
                Console.WriteLine($"讀取資料時發生錯誤: {ex.Message}");
                Console.WriteLine($"錯誤詳情: {ex}");
                _allCustomers = new List<Customer>();
            }
        }

        // 取得指定ID的客戶
        public static Customer? GetCustomerById(int id)
        {
            var customers = GetAllCustomers();
            return customers.FirstOrDefault(c => c.CustomerID == id);
        }

        // 更新客戶資料
        public static bool UpdateCustomer(Customer updatedCustomer)
        {
            try
            {
                var customers = GetAllCustomers();
                var existingCustomer = customers.FirstOrDefault(c => c.CustomerID == updatedCustomer.CustomerID);
                
                if (existingCustomer == null)
                {
                    return false;
                }
                
                // 更新客戶資料
                existingCustomer.CustomerName = updatedCustomer.CustomerName;
                existingCustomer.CustomerLocation = updatedCustomer.CustomerLocation;
                existingCustomer.Email = updatedCustomer.Email;
                existingCustomer.Phone = updatedCustomer.Phone;
                existingCustomer.Address = updatedCustomer.Address;
                
                // 保存到文件
                SaveCustomersToJson();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新客戶資料時發生錯誤: {ex.Message}");
                return false;
            }
        }

        // 新增客戶
        public static bool AddCustomer(Customer newCustomer)
        {
            try
            {
                var customers = GetAllCustomers();
                
                // 如果集合為空，設置ID為1，否則設置為最大ID+1
                if (customers.Count == 0)
                {
                    newCustomer.CustomerID = 1;
                }
                else
                {
                    newCustomer.CustomerID = customers.Max(c => c.CustomerID) + 1;
                }
                
                // 添加到客戶列表
                customers.Add(newCustomer);
                _allCustomers = customers;
                
                // 保存到JSON檔案
                SaveCustomersToJson();
                Console.WriteLine($"成功新增客戶: {newCustomer.CustomerName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"新增客戶時發生錯誤: {ex.Message}");
                return false;
            }
        }
        
        // 刪除客戶
        public static bool DeleteCustomer(int id)
        {
            try
            {
                var customers = GetAllCustomers();
                var customer = customers.FirstOrDefault(c => c.CustomerID == id);
                
                if (customer == null)
                {
                    Console.WriteLine($"找不到ID為 {id} 的客戶");
                    return false;
                }
                
                // 從列表中移除
                customers.Remove(customer);
                _allCustomers = customers;
                
                // 保存到JSON檔案
                SaveCustomersToJson();
                Console.WriteLine($"已成功刪除ID為 {id} 的客戶");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"刪除客戶時發生錯誤: {ex.Message}");
                return false;
            }
        }

        // 保存客戶資料到JSON檔案
        private static bool SaveCustomersToJson()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true, // 使JSON格式化輸出，便於閱讀
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 避免將中文轉為Unicode編碼
                };
                
                var data = new CustomerData { Customers = _allCustomers ?? new List<Customer>() };
                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(jsonFilePath, jsonString, Encoding.UTF8);
                Console.WriteLine("成功保存客戶資料到JSON檔案");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存客戶資料時發生錯誤: {ex.Message}");
                return false;
            }
        }
    }
}