using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Services;
using System;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = "x-cookie-auth";
        o.LoginPath = "/Auth/Login";
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("admin", b =>
    {
        b.RequireRole("admin");
        b.RequireClaim(ClaimTypes.Email);
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BaseAuthServices>();
builder.Services.AddScoped<TenantServices>();
builder.Services.AddSingleton<DataContextFactory>();
builder.Services.AddDbContext<GlobalDataContext>(b =>
{
    //b.UseSqlite($@"Data Source=global.db");
    var connString = builder.Configuration.GetConnectionString("Template")!.Replace("{DBNAME}", "multitenancy_global_db");
    Console.WriteLine("Using Global DB - " + connString);
    b.UseMySql(connString, ServerVersion.AutoDetect(connString));
});

//builder.Services.AddScoped(provider =>
//{
//    var ctx = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
//    if (ctx?.User?.Identity?.IsAuthenticated ?? false)
//    {
//        return new DataContext(new DbContextOptions<DataContext>(), ctx?.User?.Claims?.FirstOrDefault(x => x!.Type == "TenantId")!.Value, false);
//    }
//    return new DataContext(new DbContextOptions<DataContext>(), shared: false);
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    DatabaseMigration(app);
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void DatabaseMigration(WebApplication app)
{
    using var dbContext = app.Services.GetRequiredService<DataContextFactory>().Create("multitenancy_shared_db");
    var created = dbContext.Database.EnsureCreated();

    var connString = builder.Configuration.GetConnectionString("Template")!.Replace("{DBNAME}", "multitenancy_global_db");
    var options = new DbContextOptionsBuilder<GlobalDataContext>()
        .UseMySql(connString, ServerVersion.AutoDetect(connString))
        .Options;
    using var dbContext2 = new GlobalDataContext(options);
    var created2 = dbContext2.Database.EnsureCreated();
}