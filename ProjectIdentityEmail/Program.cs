using Microsoft.AspNetCore.Identity;
using ProjectIdentityEmail.Context;
using ProjectIdentityEmail.Entities;
using ProjectIdentityEmail.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný ve Identity Servisleri
builder.Services.AddDbContext<EmailContext>();
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<EmailContext>()
    .AddErrorDescriber<CustomIdentityValidator>();

// 2. Cookie Ayarlarý (MUTLAKA builder.Build()'den ÖNCE olmalý)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/UserLogin/";
    options.AccessDeniedPath = "/Login/UserLogin/";
    options.LogoutPath = "/Login/Logout/";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. Middleware Katmanlarý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Önce kimlik doðrulama
app.UseAuthorization();  // Sonra yetkilendirme

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 4. Uygulamayý Baþlat (En sonda olmalý)
app.Run();