using DataAccessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repo.IRepository;
using Repo.Repository;
using Services.IService;
using Services.Service;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Lấy cấu hình JWT từ appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

// Cấu hình JWT Bearer Authentication cho Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MLHR API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Nhập JWT Token theo định dạng: Bearer YOUR_TOKEN_HERE",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] {} }
    });
});

// Thêm Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role, // ✅ Đảm bảo Role đọc đúng
        ClockSkew = TimeSpan.Zero // ✅ Không cho phép thời gian trễ
    };
});

builder.Services.AddAuthorization();

// ✅ Đọc chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MinhLongDbContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Đăng ký Services và Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();

builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();

builder.Services.AddScoped<ITaxConfigRepository, TaxConfigRepository>();
builder.Services.AddScoped<ITaxConfigService, TaxConfigService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IWarehouseReceiptRepository, WarehouseReceiptRepository>();
builder.Services.AddScoped<IWarehouseReceiptService, WarehouseReceiptService>();

builder.Services.AddScoped<IRequestProductRepository, RequestProductRepository>(); 
builder.Services.AddScoped<IRequestProductService, RequestProductService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IExportRepository, ExportRepository>();
builder.Services.AddScoped<IRequestExportService, RequestExportService>();

builder.Services.AddScoped<IExportWarehouseReceiptService, ExportWarehouseReceiptService>();
builder.Services.AddScoped<IExportWarehouseReceiptRepository, ExportWarehouseReceiptRepository>();

builder.Services.AddScoped<IBatchRepository, BatchRepository>();
builder.Services.AddScoped<IBatchService, BatchService>();

builder.Services.AddScoped<IWarehouseRequestExportRepository, WarehouseRequestExportRepository>();
builder.Services.AddScoped<IWarehouseRequestExportService, WarehouseRequestExportService>();

builder.Services.AddScoped<JwtService>();
builder.Services.AddHttpContextAccessor();


// ✅ Đăng ký Controllers với JSON Options
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.WriteIndented = true; // Nếu muốn JSON format đẹp hơn
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


//134
app.UseSwagger();
app.UseSwaggerUI();


// ✅ Bật HTTPS
app.UseHttpsRedirection();

app.UseRouting();

// ✅ Bật CORS
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
// ✅ Bật Authorization
app.UseAuthorization();

// ✅ Kích hoạt API Controllers
app.MapControllers();

app.Run();
