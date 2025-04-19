using Microsoft.EntityFrameworkCore;
using WebCWMS.Models;
using WebCWMS.Repository.Implementation;
using WebCWMS.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
using WebCWMS.Repository.Implementation.WebCWMS.Repository;
using Microsoft.AspNetCore.Http.Features;
using WebCWMS.Helpers;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NewConnection")));


builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>();
builder.Services.Configure<FormOptions>(options =>
{
	options.MultipartBodyLengthLimit = 104857600; // 100MB total upload limit
	options.ValueCountLimit = int.MaxValue;
});



builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IAvenueRepository, AvenueRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IReplyRepository, ReplyRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITimeRecordRepository, TimeRecordRepository>();










var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
app.MapRazorPages();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");





var env = builder.Environment;
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	SeedSuperAdmin(services, env).Wait();
}
app.Run();

async Task SeedSuperAdmin(IServiceProvider serviceProvider, IWebHostEnvironment env)
{
	var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
	var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

	string[] roleNames = { "Superadmin", "User" };
	foreach (var roleName in roleNames)
	{
		var roleExist = await roleManager.RoleExistsAsync(roleName);
		if (!roleExist)
		{
			await roleManager.CreateAsync(new IdentityRole(roleName));
		}
	}

	var superAdmin = new ApplicationUser
	{
		UserName = "superadmin@example.com",
		Email = "superadmin@example.com",
		IsApproved = true,
		ProfilePicture = WebCWMS.Helpers.UtilityMethods.GetDefaultProfilePicture(env.WebRootPath)
	};

	var user = await userManager.FindByEmailAsync(superAdmin.Email);
	if (user == null)
	{
		var createPowerUser = await userManager.CreateAsync(superAdmin, "Superadmin123!");
		if (createPowerUser.Succeeded)
		{
			await userManager.AddToRoleAsync(superAdmin, "Superadmin");
		}
	}

}
