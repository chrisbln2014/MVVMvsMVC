using Microsoft.AspNetCore.Mvc;
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
        
        // 處理新增客戶的 POST 請求
        public IActionResult OnPostAddCustomer()
        {
            if (ModelState.IsValid)
            {
                // 調用 ViewModel 的方法來添加客戶
                bool result = CustomerVM.AddCustomer();
                
                if (result)
                {
                    // 使用 TempData 在頁面重定向後顯示成功消息
                    TempData["SuccessMessage"] = "客戶已成功新增";
                    // 重定向到 GET 請求以避免重複提交
                    return RedirectToPage();
                }
            }
            
            // 如果添加失敗，保留當前頁面狀態並顯示錯誤消息
            return Page();
        }
        
        // 處理刪除客戶的 POST 請求
        public IActionResult OnPostDeleteCustomer(int id)
        {
            // 調用 ViewModel 的方法來刪除客戶
            bool result = CustomerVM.DeleteCustomer(id);
            
            if (result)
            {
                // 使用 TempData 在頁面重定向後顯示成功消息
                TempData["SuccessMessage"] = "客戶已成功刪除";
            }
            else
            {
                // 使用 TempData 在頁面重定向後顯示錯誤消息
                TempData["ErrorMessage"] = "刪除客戶失敗";
            }
            
            // 重定向到 GET 請求以重新載入頁面
            return RedirectToPage();
        }
    }
}