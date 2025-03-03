using System.Text.Json;
using System.Text;

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

        // 靜態方法：獲取所有客戶資料
        public static List<Customer> GetAllCustomers()
        {
            // 如果快取中沒有資料，則從 JSON 檔案讀取
            if (_allCustomers == null)
            {
                try
                {
                    // 指定 JSON 檔案的完整路徑
                    string jsonFilePath = @"C:\Users\1418\Documents\projects\MVVMvsMVC\Data\customers.json";

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
            return _allCustomers ?? new List<Customer>();
        }
    }


}