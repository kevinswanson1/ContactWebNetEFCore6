using ContactWebNetEFCore6.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyContactManagerData;
using MyContactManagerRepositories;
using MyContactManagerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - Allowing dependency injection of contexts into controllers

// Add ApplicationDbContext connection to the WebApplication Service Collection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

// Add MyContactManagerDbContext connection to the WebApplication Service Collection
var mcmdContext = builder.Configuration.GetConnectionString("MyContactManager");
builder.Services.AddDbContext<MyContactManagerDbContext>(options =>
	options.UseSqlServer(mcmdContext));

// Add Entity Framework migrations page exception filter for DB exception problem solving
// during development 
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add common identity services requiring a confirmed user account to sign in
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>();

// Add services for controllers with views
builder.Services.AddControllersWithViews();

// FIX: InvalidOperationException: Unable to resolve service for type 'MyContactManagerServices.IStatesService'
// while attempting to activate 'ContactWebNetEFCore6.Controllers.StatesController'.
// Consider the order of the Builder Services AddScoped command statements. Since in the layers, the services
// reference the repositories, so we want to build them first.
builder.Services.AddScoped<IStatesRepository, StatesRepository>();
builder.Services.AddScoped<IStatesService, StatesService>();
builder.Services.AddScoped<IContactsRepository, ContactsRepository>();
builder.Services.AddScoped<IContactsService, ContactsService>();

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

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
