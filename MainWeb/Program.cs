// 建立 Web 應用程式建構器，這是所有 ASP.NET Core 應用程式的起點
var builder = WebApplication.CreateBuilder(args);

// 添加 MVC (Model-View-Controller) 服務
// MVC 是一種將應用程式分為三個主要組件的設計模式：
// - Model（模型）：處理資料和業務邏輯
// - View（視圖）：負責使用者介面
// - Controller（控制器）：協調模型和視圖之間的互動
builder.Services.AddControllersWithViews();

// 添加 Razor Pages 服務，用於 MVVM (Model-View-ViewModel) 架構
// Razor Pages 是一種更簡單的頁面導向模式，更接近 MVVM 架構
// 每個頁面都有自己的模型和處理邏輯
builder.Services.AddRazorPages();

// 使用上述配置建立應用程式實例
var app = builder.Build();

// 環境判斷：如果不是開發環境（例如是生產環境）
// 則將所有未處理的異常導向到 /Error 頁面
// 在開發環境中，會顯示更詳細的錯誤信息，有助於除錯
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// 啟用靜態檔案服務
// 這使應用程式能夠提供靜態檔案如 HTML、CSS、JavaScript、圖片等
// 這些檔案通常存放在 wwwroot 資料夾中
app.UseStaticFiles();

// 啟用路由功能
// 路由決定了如何根據 URL 將請求導向到正確的處理程序
app.UseRouting();

// 配置 MVC 路由，使用 "mvc" 作為路由前綴
// 例如：/mvc/Home/Index 會路由到 HomeController 的 Index 方法
// {controller=Home} 代表如果 URL 沒有指定控制器，預設使用 HomeController
// {action=Index} 代表如果 URL 沒有指定動作，預設使用 Index 方法
// {id?} 表示 id 參數是可選的
app.MapControllerRoute(
    name: "mvc",
    pattern: "mvc/{controller=Home}/{action=Index}/{id?}");

// 配置 Razor Pages (MVVM) 路由
// Razor Pages 的路由是自動根據檔案結構映射的
// 例如：/Contact 會映射到 Pages/Contact.cshtml
// 例如：/Customers/Details 會映射到 Pages/Customers/Details.cshtml
app.MapRazorPages();

// 當沒有匹配的路由時，預設回到 Index 頁面
// 這確保用戶不會看到 404 錯誤頁面，而是被重定向到首頁
app.MapFallbackToPage("/Index");

// 啟動應用程式，開始監聽和處理 HTTP 請求
app.Run();
