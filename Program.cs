using Demo.Controllers.Management.Authentication;
using Demo.Controllers.Management.CrudManagement;
using Demo.Controllers.utilities;

var builder = WebApplication.CreateBuilder(args);

// Đọc cấu hình từ appsettings.json
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();  // Đảm bảo có thể truy cập HttpContext
builder.Services.AddSession(options =>
{
    // Cấu hình session với các tùy chọn (optional)
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian hết hạn session
    options.Cookie.IsEssential = true;  // Đảm bảo cookie sẽ luôn được gửi cùng với yêu cầu
    options.Cookie.HttpOnly = true; // Đảm bảo cookie chỉ có thể truy cập thông qua HTTP (không qua JS)
});

// Đăng ký CSVServices với Dependency Injection
builder.Services.AddSingleton<CSVServices>(serviceProvider =>
{
    var userCsvFilePath = configuration["CsvFiles:Users"] ?? "users.csv";
    var courseCsvFilePath = configuration["CsvFiles:Courses"] ?? "courses.csv";
    var gradeCsvFilePath = configuration["CsvFiles:Grades"] ?? "grades.csv";
    return new CSVServices(userCsvFilePath, courseCsvFilePath, gradeCsvFilePath);
});

// Đăng ký AuthManagement
builder.Services.AddSingleton<AuthManagement>();
builder.Services.AddScoped<UpdateUser>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Account/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Thêm middleware Session và Authentication/Authorization
app.UseSession(); // Đảm bảo sử dụng session trong pipeline
app.UseAuthentication();  // Thêm authentication nếu có đăng nhập
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
