using Blazor_Server.Data;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 添加核心服務
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 添加 Blazor Server 服務，不使用不支援的 PathBase 選項
builder.Services.AddServerSideBlazor();

// 添加客戶服務
builder.Services.AddScoped<CustomerService>();

// 添加 HttpClient 服務以便在需要時使用
builder.Services.AddHttpClient();

var app = builder.Build();

// 環境配置
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 設置基本靜態檔案服務
app.UseStaticFiles();

// 添加對 Data 目錄的訪問，允許直接訪問 json 檔案
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "..", "Data")),
    RequestPath = "/data"
});

// 確保 Blazor 相關資源正確載入
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "..", "Blazor-Server", "wwwroot")),
    RequestPath = "/_content/Blazor-Server"
});

// Blazor WebAssembly 的靜態資源配置
// 重要：使用新的 StaticFileOptions 配置，禁用資源完整性檢查
var webAssemblyWwwrootPath = Path.Combine(builder.Environment.ContentRootPath, "..", "Blazor-WebAssembly", "bin", "Debug", "net8.0", "wwwroot");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(webAssemblyWwwrootPath),
    RequestPath = "/blazor-wasm",
    OnPrepareResponse = ctx =>
    {
        // 停用快取，確保始終獲取最新資源
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
        ctx.Context.Response.Headers.Append("Expires", "-1");
        
        // 移除任何內容安全策略標頭，它可能會阻止某些資源載入
        if (ctx.Context.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            ctx.Context.Response.Headers.Remove("Content-Security-Policy");
        }
    }
});

// 將常規的 Blazor-WebAssembly/wwwroot 內容映射為靜態檔案
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "..", "Blazor-WebAssembly", "wwwroot")),
    RequestPath = "/blazor-wasm-content"
});

// 打印關鍵路徑以便確認配置
Console.WriteLine($"WebAssembly 編譯輸出路徑: {webAssemblyWwwrootPath}");
Console.WriteLine($"_framework 目錄是否存在: {Directory.Exists(Path.Combine(webAssemblyWwwrootPath, "_framework"))}");

app.UseRouting();
app.UseAuthorization();

// 使用頂層路由註冊

// 1. 設定 MVC 控制器路由
app.MapControllerRoute(
    name: "mvc",
    pattern: "mvc/{controller=Home}/{action=Index}/{id?}");
    
// 2. 配置 Blazor Hub (SignalR 連接點)
app.MapBlazorHub("/blazor-server/_blazor");

// 3. 配置 Razor Pages
app.MapRazorPages();

// 4. 配置首頁路由
app.MapGet("/", context => {
    context.Response.Redirect("/Index");
    return System.Threading.Tasks.Task.CompletedTask;
});

// 5. Blazor Server 應用的路由
app.MapFallbackToPage("/blazor-server/{**path}", "/blazor-server/_Host");

// 6. Blazor WebAssembly 應用的路由
app.MapFallbackToPage("/blazor-wasm/{**path}", "/blazor-wasm/_Host");

app.Run();
