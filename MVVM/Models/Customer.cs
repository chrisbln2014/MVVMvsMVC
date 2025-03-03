using System.Text.Json;
using System.Text;

namespace MVVM.Models
{
    public class Customer
    {
        public required int CustomerID { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerLocation { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        private static List<Customer>? _allCustomers;

        public static List<Customer> GetAllCustomers()
        {
            if (_allCustomers == null)
            {
                try
                {
                    // 使用絕對路徑
                    string jsonFilePath = @"C:\Users\1418\Documents\projects\MVVMvsMVC\Data\customers.json";

                    Console.WriteLine($"JSON 文件路徑: {jsonFilePath}");

                    if (File.Exists(jsonFilePath))
                    {
                        Console.WriteLine("找到 JSON 文件，開始讀取...");
                        string jsonString = File.ReadAllText(jsonFilePath, Encoding.UTF8);
                        Console.WriteLine($"JSON 內容: {jsonString}"); // 輸出 JSON 內容

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true // 忽略屬性名稱的大小寫
                        };

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
                    Console.WriteLine($"讀取資料時發生錯誤: {ex.Message}");
                    Console.WriteLine($"錯誤詳情: {ex}");
                    _allCustomers = new List<Customer>();
                }
            }
            return _allCustomers ?? new List<Customer>();
        }
    }

    internal class CustomerData
    {
        public List<Customer> Customers { get; set; } = new();
    }
}