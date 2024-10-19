using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PizzaCompany.Data;
using PizzaCompany.DTOs;
using PizzaCompany.Extensions;
using PizzaCompany.Filters.ActionFilters;
using PizzaCompany.Interface;
using PizzaCompany.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the AuditInterceptor
builder.Services.AddScoped<AuditInterceptor>();

// Register DbContext with the interceptor
builder.Services.AddDbContext<PizzaCompanyDbContext>((serviceProvider, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr"));
    options.AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
});

//Register Repositories
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<IRoleRepository, RoleRepository>();
//builder.Services.AddScoped<IRoleChangeRequestRepository, RoleChangeRequestRepository>();

//Register Unit of Work
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

//Register Password Hasher
builder.Services.AddScoped<IPasswordHasher,PasswordHasher>();

// Register the background service
builder.Services.AddHostedService<ScheduledDataService>();

// Register the ActionFilter
builder.Services.AddScoped<User_ValidateExistingUserFilterAttribute>();
builder.Services.AddScoped<Product_ValidateExistingProductFilterAttribue>();
builder.Services.AddScoped<Product_ValidateProductIdFilterAttribute>();
builder.Services.AddScoped<RoleChangeRequest_ValidateIdFilterAttribute>();

// Register Validator
builder.Services.AddScoped<IValidator<RoleChangeRequestDTO>, RoleChangeRequestDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateRoleRequestDTO>, UpdateRoleRequestDTOValidator>();

// Register HttpContextAccessor to use HttpContext OUTSIDE Controller class
builder.Services.AddHttpContextAccessor();

// Add services to the container and configure Newtonsoft.Json for handling reference loops
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Register JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]))
        };
    });

var app = builder.Build();

// Add the custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization(); 

app.MapControllers();

app.Run();