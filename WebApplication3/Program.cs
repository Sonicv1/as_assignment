using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;
using reCAPTCHA.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddDataProtection();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "ExampleSession";
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.IsEssential = true;
});

builder.Services.AddRecaptcha(options =>
{
    options.SiteKey = builder.Configuration["6LcrHVUkAAAAAAN1rTeIdwhz_79rg2HIXTL4QQeA"];
    options.SecretKey = builder.Configuration["6LcrHVUkAAAAAKtjeCyNflPJI4Irr0pqvpYLtPJa"];
});

builder.Services.AddRecaptchaService();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromSeconds(30);

    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Index";
    options.SlidingExpiration = true;

});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseStatusCodePagesWithReExecute("/error");

app.UseAuthorization();

app.MapRazorPages();

app.Run();
