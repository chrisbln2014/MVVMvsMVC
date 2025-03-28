namespace Blazor_WebAssembly.Models
{
    // 客戶資料模型類別，代表單個客戶的資料結構
    public class Customer
    {
        public int CustomerID { get; set; }            // 客戶編號，唯一識別碼
        public string CustomerName { get; set; } = "";  // 客戶名稱
        public string CustomerLocation { get; set; } = ""; // 客戶所在地
        public string? Email { get; set; }             // 電子郵件（可為空）
        public string? Phone { get; set; }             // 電話（可為空）
        public string? Address { get; set; }           // 地址（可為空）
    }

    // 用於JSON反序列化的容器類
    public class CustomerData
    {
        public List<Customer> Customers { get; set; } = new();
    }
}