using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Collections.Generic;

namespace MVC.Controllers
{
    // 客戶控制器：處理與客戶相關的所有 HTTP 請求
    // 在 MVC 模式中，控制器負責處理用戶請求並回應適當的視圖
    public class CustomerController : Controller
    {
        // HTTP GET 請求的處理方法
        // 當用戶訪問客戶列表頁面時會調用此方法
        public IActionResult Index()
        {
            // 從服務層獲取客戶資料
            List<Customer> customers = CustomerService.GetAllCustomers();
            
            // 將資料傳遞給視圖
            // View() 方法會自動尋找對應的視圖文件 (Index.cshtml)
            return View(customers);
        }
    }
}