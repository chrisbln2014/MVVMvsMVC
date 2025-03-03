using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVM.ViewModels;

namespace MVVM.Pages
{
    public class CustomersModel : PageModel
    {
        public CustomerViewModel CustomerVM { get; private set; }

        public CustomersModel()
        {
            CustomerVM = new CustomerViewModel();
        }

        public void OnGet()
        {
            // 確保在頁面加載時重新讀取最新資料
            CustomerVM = new CustomerViewModel();
        }
    }
}