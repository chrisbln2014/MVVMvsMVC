// 這是主要入口網站的程式進入點
// 負責配置和啟動整個應用程式
var builder = WebApplication.CreateBuilder(args);

// 添加 MVC (Model-View-Controller) 服務
// MVC 是一種將應用程式分為三個主要組件的設計模式：
// - Model（模型）：處理資料和業務邏輯
// - View（視圖）：負責使用者介面
// - Controller（控制器）：協調模型和視圖之間的互動
builder.Services.AddControllersWithViews();

// 加入 Razor Pages 服務
// MainWeb 使用 Razor Pages 作為首頁介面
builder.Services.AddRazorPages();

// 使用上述配置建立應用程式實例
var app = builder.Build();

// 環境判斷：如果不是開發環境（例如是生產環境）
// 則將所有未處理的異常導向到 /Error 頁面
// 在開發環境中，會顯示更詳細的錯誤信息，有助於除錯
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // 啟用 HTTPS 重定向
    app.UseHsts();
}

// 將 HTTP 請求重定向到 HTTPS
app.UseHttpsRedirection();

// 啟用靜態檔案服務
// 這使應用程式能夠提供靜態檔案如 HTML、CSS、JavaScript、圖片等
// 這些檔案通常存放在 wwwroot 資料夾中
app.UseStaticFiles();

// 啟用路由功能
// 路由決定了如何根據 URL 將請求導向到正確的處理程序
app.UseRouting();

// 啟用授權功能
app.UseAuthorization();

// 配置 MVC 路由，使用 "mvc" 作為路由前綴
// 例如：/mvc/Home/Index 會路由到 HomeController 的 Index 方法
// {controller=Home} 代表如果 URL 沒有指定控制器，預設使用 HomeController
// {action=Index} 代表如果 URL 沒有指定動作，預設使用 Index 方法
// {id?} 表示 id 參數是可選的
app.MapControllerRoute(
    name: "mvc",
    pattern: "mvc/{controller=Home}/{action=Index}/{id?}");

// 設定使用 Razor Pages 的路由規則
// 這會將請求映射到 Pages 資料夾中的對應頁面
app.MapRazorPages();

// 當沒有匹配的路由時，預設回到 Index 頁面
// 這確保用戶不會看到 404 錯誤頁面，而是被重定向到首頁
app.MapFallbackToPage("/Index");

// 啟動應用程式，開始監聽和處理 HTTP 請求
app.Run();
