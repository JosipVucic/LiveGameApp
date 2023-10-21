using LiveGameApp.Models;
using LiveGameApp.Services;
using LiveGameApp.Hubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LiveGameAppContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("MyConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Appuser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<Role>()
    .AddEntityFrameworkStores<LiveGameAppContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<Appuser, LiveGameAppContext>()
    .AddProfileService<MyProfileService>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

builder.Services.AddControllers().AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null); ;
builder.Services.AddRazorPages();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:44494")
            .WithExposedHeaders("*")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");*/

//app.MapDefaultControllerRoute();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.MapRazorPages();

app.MapFallbackToFile("index.html"); ;

CreateRoles().Wait();

app.Run();

async Task CreateRoles()
{
    IServiceProvider serviceProvider = app.Services.CreateScope().ServiceProvider;
    var RoleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
    var UserManager = serviceProvider.GetRequiredService<UserManager<Appuser>>();

    string[] roleNames = { "Admin", "Moderator", "User" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await RoleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await RoleManager.CreateAsync(new Role { Name = roleName });
        }
    }

    var administrator = new Appuser
    {
        UserName = builder.Configuration.GetValue<string>("AdminUserName"),
        Email = builder.Configuration.GetValue<string>("AdminUserEmail"),
        DateOfBirth = DateTime.MinValue,
    };
    //Ensure you have these values in your appsettings.json file
    string userPWD = builder.Configuration.GetValue<string>("AdminUserPassword");
    var _user = await UserManager.FindByEmailAsync(administrator.Email);

    if (_user == null)
    {

        var createPowerUser = await UserManager.CreateAsync(administrator, userPWD);

        if (createPowerUser.Succeeded)
        {
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(administrator);
            await UserManager.ConfirmEmailAsync(administrator, token);

            //here we tie the new user to the role
            await UserManager.AddToRoleAsync(administrator, "Admin");
        }
    }

}
