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

        // MVC模式特點：Get方法用於顯示編輯表單，Post方法用於處理表單提交
        // 所有資料處理邏輯都集中在Controller中
        
        // GET: Customer/Edit/5
        // 顯示編輯表單，Controller負責取得數據並傳給View
        public IActionResult Edit(int id)
        {
            // MVC特點：Controller直接從Model層取得資料
            var customer = CustomerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }

            // MVC特點：將Model直接傳遞給View，由View顯示
            return View(customer);
        }

        // POST: Customer/Edit/5
        // 處理編輯表單的提交
        // MVC特點：表單數據通過HTTP POST提交到Controller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("CustomerID,CustomerName,CustomerLocation,Email,Phone,Address")] Customer customer)
        {
            // 確保ID匹配
            if (id != customer.CustomerID)
            {
                return NotFound();
            }

            // MVC特點：所有的驗證和業務邏輯都在Controller中處理
            if (ModelState.IsValid)
            {
                // MVC特點：Controller調用Model層的方法更新數據
                bool result = CustomerService.UpdateCustomer(customer);
                if (result)
                {
                    // 設置臨時數據用於顯示成功消息
                    TempData["SuccessMessage"] = "客戶資料已成功更新";
                    // 重定向到Index頁面（避免重複提交表單）
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // 添加錯誤消息到ModelState
                    ModelState.AddModelError("", "更新客戶資料失敗");
                }
            }

            // 如果验证失败，返回帶有错误信息的表单视图
            return View(customer);
        }
        
        // GET: Customer/Create
        // 顯示創建新客戶的表單
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: Customer/Create
        // 處理創建新客戶的表單提交
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CustomerName,CustomerLocation,Email,Phone,Address")] Customer customer)
        {
            // 驗證模型狀態
            if (ModelState.IsValid)
            {
                // 添加新客戶
                bool result = CustomerService.AddCustomer(customer);
                
                if (result)
                {
                    // 設置成功消息
                    TempData["SuccessMessage"] = "客戶已成功創建";
                    // 重定向到客戶列表頁面
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // 添加錯誤消息
                    ModelState.AddModelError("", "創建客戶失敗");
                }
            }
            
            // 如果驗證失敗，返回表單視圖
            return View(customer);
        }
        
        // GET: Customer/Delete/5
        // 顯示刪除確認頁面
        public IActionResult Delete(int id)
        {
            // 獲取要刪除的客戶
            var customer = CustomerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            
            // 返回刪除確認視圖
            return View(customer);
        }
        
        // POST: Customer/Delete/5
        // 處理刪除確認
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // 執行刪除操作
            bool result = CustomerService.DeleteCustomer(id);
            
            if (result)
            {
                // 設置成功消息
                TempData["SuccessMessage"] = "客戶已成功刪除";
            }
            else
            {
                // 設置錯誤消息
                TempData["ErrorMessage"] = "刪除客戶失敗";
            }
            
            // 重定向到客戶列表頁面
            return RedirectToAction(nameof(Index));
        }
    }
}