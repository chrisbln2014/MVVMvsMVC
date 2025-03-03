// 這是 ASP.NET Core 應用程式的入口點文件
// WebApplication.CreateBuilder 用於創建應用程式的基礎建設
var builder = WebApplication.CreateBuilder(args);

// 將 Razor Pages 服務加入到依賴注入容器中
// Razor Pages 是 MVVM 模式的實現方式，用於建立網頁UI
builder.Services.AddRazorPages();

// 建立應用程式實例
var app = builder.Build();

// 配置 HTTP 請求處理管道
// 如果不是開發環境，則使用錯誤處理頁面
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// 啟用靜態文件服務（如 CSS、JavaScript、圖片等）
app.UseStaticFiles();

// 啟用路由功能
app.UseRouting();

// 啟用授權功能（用於處理用戶權限）
app.UseAuthorization();

// 設定使用 Razor Pages 的路由
app.MapRazorPages();

// 啟動應用程式
app.Run();
