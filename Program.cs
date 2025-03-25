using Demo.Controllers.Management.Authentication;
using Demo.Controllers.utilities;

var builder = WebApplication.CreateBuilder(args);

// Đọc cấu hình từ appsettings.json
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

// Đăng ký CSVServices với Dependency Injection
builder.Services.AddSingleton<CSVServices>(serviceProvider =>
{
    var userCsvFilePath = configuration["CsvFiles:Users"] ?? "users.csv";
    var courseCsvFilePath = configuration["CsvFiles:Courses"] ?? "courses.csv";
    return new CSVServices(userCsvFilePath, courseCsvFilePath);
});
// Đăng ký AuthManagement
builder.Services.AddSingleton<AuthManagement>();
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
app.UseSession();
app.UseAuthentication(); // Thêm authentication nếu có đăng nhập
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");
app.Run();