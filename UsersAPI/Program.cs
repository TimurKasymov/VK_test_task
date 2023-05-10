using DLL;
using DLL.Abstractions;
using DLL.Entities;
using DLL.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Authentication;
using UsersAPI.Services;
using UsersAPI.Services.EntityServices;
using UsersAPI.Services.EntityServices.DI;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<UsersContext>(options =>
{
    options.UseNpgsql(connectionString);
});
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserGroupService, UserGroupService>();
builder.Services.AddScoped<IUserStateService, UserStateService>();

builder.Services.AddScoped<IDbRepository<User>, UserRepository>();
builder.Services.AddScoped<IDbRepository<UserGroup>, UserGroupRepository>();
builder.Services.AddScoped<IDbRepository<UserState>, UserStateRepository>();
builder.Services.AddSingleton<HashService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();

public partial class Program { }