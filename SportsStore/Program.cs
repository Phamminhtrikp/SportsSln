using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Config Razor pages
builder.Services.AddRazorPages();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Config db
builder.Services.AddDbContext<StoreDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:SportsStoreConnection"]);
});

builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Active Blazor 
builder.Services.AddServerSideBlazor();

builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(
    builder.Configuration["ConnectionStrings:IdentityConnection"]
));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>();



var app = builder.Build();

// Configuration handle error
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/error");
}

app.UseRequestLocalization(opts =>
{
    opts.AddSupportedCultures("en-US")
    .AddSupportedUICultures("en-US")
    .SetDefaultCulture("en-US");
});


// app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();


app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute("catpage", "{category}/Page{productPage:int}", new { Controller = "Home", action = "Index" });
app.MapControllerRoute("page", "Page{productPage:int}", new { Controller = "Home", action = "Index", productPage = 1 });
app.MapControllerRoute("category", "{category}", new { Controller = "Home", action = "Index", productPage = 1 });
app.MapControllerRoute("pagination", "Products/Page{productPage}", new { Controller = "Home", action = "Index", productPage = 1 });


/*app.MapControllerRoute("Pagination", "Products/Page{productPage}", new {Controller = "Home", action = "Index"});*/

app.MapDefaultControllerRoute();

// Register Razor pages
app.MapRazorPages();


// Blazor
app.MapBlazorHub();
app.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");

SeedData.EnsurePopulated(app);

IdentitySeedData.EnsurePopulated(app);

app.Run();
