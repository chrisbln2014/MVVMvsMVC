// 這是 ASP.NET Core MVC 應用程式的入口點文件
// 創建應用程式的基礎建設
var builder = WebApplication.CreateBuilder(args);

// 將 MVC 服務加入到依賴注入容器中
// 這會啟用 MVC 的核心功能，包括控制器和視圖的支援
builder.Services.AddControllersWithViews();

// 建立應用程式實例
var app = builder.Build();

// 配置 HTTP 請求處理管道
// 如果不是開發環境，則使用錯誤處理
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// 啟用靜態文件服務（CSS、JavaScript、圖片等）
app.UseStaticFiles();

// 啟用路由功能
app.UseRouting();

// 啟用授權功能
app.UseAuthorization();

// 設定 MVC 的預設路由規則
// {controller} - 控制器名稱，預設為 Customer
// {action} - 動作方法名稱，預設為 Index
// {id?} - 可選的 ID 參數
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Index}/{id?}");

// 啟動應用程式
app.Run();
