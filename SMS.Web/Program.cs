using SMS.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Infrastructure.Services;
using SMS.Infrastructure.AutoMapper;
using SMS.Data.Models;
using SMS.Infrastructure.Services.Categories;
using Microsoft.Extensions.Options;
using SMS.Infrastructure.Services.Advertisements;
using SMS.Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CMSDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User,IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<CMSDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IAdvertisementService, AdvertisementService>();
builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(opts => opts.UseMiddleware<ExceptionHandler>());
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=user}/{action=index}/{id?}");
app.MapRazorPages();

app.Run();
