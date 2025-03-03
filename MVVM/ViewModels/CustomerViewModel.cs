using MVVM.Models;

namespace MVVM.ViewModels
{
    // ViewModel 負責處理視圖（View）所需的資料和邏輯
    // 這是 MVVM 模式的核心組件之一，負責連接 Model 和 View
    public class CustomerViewModel
    {
        // 儲存客戶資料的集合，只提供讀取權限
        // private set 表示只能在類別內部修改
        public List<Customer> Customers { get; private set; }

        // 建構函式：當創建 ViewModel 實例時執行
        public CustomerViewModel()
        {
            try
            {
                // 從 Model 層獲取客戶資料
                Customers = Customer.GetAllCustomers();
                Console.WriteLine($"CustomerViewModel 成功載入了 {Customers.Count} 個客戶資料");
            }
            catch (Exception ex)
            {
                // 如果發生錯誤，建立空列表並記錄錯誤
                Console.WriteLine($"CustomerViewModel 載入資料時發生錯誤: {ex.Message}");
                Customers = new List<Customer>();
            }
        }
    }
}