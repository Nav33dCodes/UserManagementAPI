using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Services;
using UserManagementAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Register EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

//Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<UserManagementAPI.Mappings.MappingProfile>());

// Register UserService
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();