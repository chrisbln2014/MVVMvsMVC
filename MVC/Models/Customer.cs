using System.Text.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.IO;

namespace MVC.Models
{
    // 客戶資料模型：定義客戶的基本資料結構
    // 在 MVC 架構中，這是 Model 層的一部分
    public class Customer
    {
        // required 表示這些屬性必須要有值，不能為 null
        public required int CustomerID { get; set; }           // 客戶編號（唯一識別碼）
        public required string CustomerName { get; set; }      // 客戶名稱
        public required string CustomerLocation { get; set; }  // 客戶所在地
        public string? Email { get; set; }                    // 電子郵件（可為空）
        public string? Phone { get; set; }                    // 電話（可為空）
        public string? Address { get; set; }                  // 地址（可為空）
    }

    // 用於 JSON 反序列化的輔助類別
    internal class CustomerData
    {
        public List<Customer> Customers { get; set; } = new();
    }

    // 客戶服務類別：處理客戶資料的存取邏輯
    public static class CustomerService
    {
        // 快取儲存所有客戶資料
        private static List<Customer>? _allCustomers;
        
        // 使用相對路徑定義JSON檔案路徑
        private static readonly string jsonFilePath = Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName ?? "",
            "Data", 
            "customers.json");

        // 獲取所有客戶資料的方法
        public static List<Customer> GetAllCustomers()
        {
            // 如果快取中沒有資料，則從 JSON 檔案讀取
            if (_allCustomers == null)
            {
                LoadCustomersFromJson();
            }
            return _allCustomers ?? new List<Customer>();
        }
        
        // 從JSON檔案載入客戶資料
        private static void LoadCustomersFromJson()
        {
            try
            {
                Console.WriteLine($"JSON 文件路徑: {jsonFilePath}");

                if (File.Exists(jsonFilePath))
                {
                    Console.WriteLine("找到 JSON 文件，開始讀取...");
                    // 使用 UTF8 編碼讀取 JSON 檔案
                    string jsonString = File.ReadAllText(jsonFilePath, Encoding.UTF8);
                    Console.WriteLine($"JSON 內容: {jsonString}");

                    // 設定 JSON 反序列化選項
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // 屬性名稱不分大小寫
                    };

                    // 將 JSON 資料轉換為客戶物件集合
                    var data = JsonSerializer.Deserialize<CustomerData>(jsonString, options);
                    _allCustomers = data?.Customers ?? new List<Customer>();
                    Console.WriteLine($"成功讀取到 {_allCustomers.Count} 個客戶資料");
                }
                else
                {
                    // 如果檔案不存在，建立空列表
                    _allCustomers = new List<Customer>();
                    Console.WriteLine($"檔案不存在: {jsonFilePath}");
                }
            }
            catch (Exception ex)
            {
                // 錯誤處理：記錄錯誤並返回空列表
                Console.WriteLine($"讀取資料時發生錯誤: {ex.Message}");
                Console.WriteLine($"錯誤詳情: {ex}");
                _allCustomers = new List<Customer>();
            }
            finally
            {
                Console.WriteLine($"_allCustomers 是否為 null: {_allCustomers == null}");
            }
        }
        
        // 根據ID取得特定客戶資料
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
                
                // 保存到JSON檔案
                SaveCustomersToJson();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新客戶資料時發生錯誤: {ex.Message}");
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
                    WriteIndented = true, // 讓JSON格式化，方便閱讀
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