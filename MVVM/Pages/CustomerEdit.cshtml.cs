using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVM.ViewModels;

namespace MVVM.Pages
{
    // 客戶資料編輯頁面的模型類
    public class CustomerEditModel : PageModel
    {
        // MVVM 模式的核心：頁面直接使用 ViewModel 而非 Model
        [BindProperty]
        public CustomerViewModel CustomerVM { get; set; }

        public CustomerEditModel()
        {
            CustomerVM = new CustomerViewModel();
        }

        // 處理 GET 請求，加載客戶資料
        public IActionResult OnGet(int id)
        {
            // 使用 ViewModel 選擇特定客戶進行編輯
            CustomerVM.SelectCustomerForEdit(id);

            // 如果找不到客戶，返回 404 頁面
            if (CustomerVM.CustomerToEdit == null)
            {
                return NotFound();
            }

            return Page();
        }

        // 處理 POST 請求，保存客戶資料
        public IActionResult OnPost()
        {
            // MVVM 特點：驗證邏輯位於 ViewModel 中
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 調用 ViewModel 的方法保存變更
            // 在 MVVM 中，所有業務邏輯處理都在 ViewModel 中進行
            bool result = CustomerVM.SaveChanges();

            if (result)
            {
                // 成功保存後返回列表頁面
                TempData["SuccessMessage"] = "客戶資料已成功更新";
                return RedirectToPage("./Customers");
            }
            else
            {
                // 發生錯誤時，錯誤訊息由 ViewModel 提供
                // 不需要在這裡添加錯誤訊息到 ModelState
                return Page();
            }
        }
    }
}