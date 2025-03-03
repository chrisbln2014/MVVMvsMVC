using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVM.ViewModels;

namespace MVVM.Pages
{
    // PageModel 是 Razor Pages 的基礎類別
    // 這個類別處理 Customers.cshtml 頁面的後端邏輯
    public class CustomersModel : PageModel
    {
        // 宣告 ViewModel 屬性，用於在頁面中顯示客戶資料
        // 這是 MVVM 模式中 View 和 ViewModel 的連接點
        public CustomerViewModel CustomerVM { get; private set; }

        // 建構函式：當頁面被首次請求時執行
        public CustomersModel()
        {
            // 創建 ViewModel 實例以獲取客戶資料
            CustomerVM = new CustomerViewModel();
        }

        // 當使用 GET 方法訪問頁面時執行的方法
        // 例如：當用戶直接訪問頁面或重新整理頁面時
        public void OnGet()
        {
            // 重新創建 ViewModel 實例以確保資料是最新的
            CustomerVM = new CustomerViewModel();
        }
    }
}