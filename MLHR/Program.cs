using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using Repo.Repository;
using Services.IService;
using Services.Service;

var builder = WebApplication.CreateBuilder(args);

// ✅ Đọc chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MinhLongDbContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Đăng ký Services và Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<IAgencyAccountRepository, AgencyAccountRepository>();
builder.Services.AddScoped<IAgencyAccountService, AgencyAccountService>();

builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();

// ✅ Đăng ký Controllers với JSON Options
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// ✅ Đăng ký HttpClient
builder.Services.AddHttpClient();

// ✅ Đăng ký Swagger để test API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Bật CORS (cho phép gọi API từ các client khác nhau)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ✅ Cấu hình Middleware cho Swagger (chỉ trong môi trường Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Bật HTTPS
app.UseHttpsRedirection();

app.UseRouting();

// ✅ Bật CORS
app.UseCors("AllowAllOrigins");

// ✅ Bật Authorization
app.UseAuthorization();

// ✅ Kích hoạt API Controllers
app.MapControllers();

app.Run();
