using FirstProject.DataAccess.Data;
using FirstProject.DataAccess.Repository;
using FirstProject.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using FirstProject.Utility;
using Stripe;
using FirstProject.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddPortableObjectLocalization();
builder.Services
    .Configure<RequestLocalizationOptions>(options => options
        .AddSupportedCultures("ar")
        .AddSupportedUICultures("ar"));
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender,EmailSender>();

//builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser,IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages().AddViewLocalization();

builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = $"/Identity/Account/Login";
        options.LogoutPath = $"/Identity/Account/Logout";
        options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    });
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(2000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication().AddFacebook(option => {
    option.AppId = "267717212648575";
    option.AppSecret = "b88fa814333a95c1410a3035e6c39d9e";
});
builder.Services.AddAuthentication().AddMicrosoftAccount(option => {
    option.ClientId = "025abc54-c66a-453d-9f8c-83c3dc44a5cb";
    option.ClientSecret = "5zc8Q~jy4wbXPK8KkSW32a-ak-QmOTT~pgWiHcKj";
});
builder.Services.AddAuthentication().AddGoogle(googleOptions => {
    googleOptions.ClientId = "372542609664-m7jvns7kl2go4u3crigr4201c06egas5.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
});
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
SeedDatabase();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}