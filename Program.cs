using CMS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
   .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("CMMCnn")));

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Manager ", "Coordinator", "HR" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}


using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Create and assign role to the "Coordinator" user
    string email = "david@coordinator.com";
    string password = "Dass1200@";

    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        user = new IdentityUser { UserName = email, Email = email };

        // Create the user
        var createUserResult = await userManager.CreateAsync(user, password);

        if (createUserResult.Succeeded)
        {
            // If user creation is successful, add the user to the "Coordinator" role
            await userManager.AddToRoleAsync(user, "Coordinator");
        }
        else
        {
            // Handle user creation failure (optional: log the errors)
            foreach (var error in createUserResult.Errors)
            {
                Console.WriteLine($"Error creating user: {error.Description}");
            }
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Create and assign role to the "Manager" user
    string email = "david@manager.com";
    string password = "Diddy1234!";

    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        user = new IdentityUser { UserName = email, Email = email };

        // Create the user
        var createUserResult = await userManager.CreateAsync(user, password);

        if (createUserResult.Succeeded)
        {
            // If user creation is successful, add the user to the "Manager" role
            await userManager.AddToRoleAsync(user, "Manager");
        }
        else
        {
            // Handle user creation failure (optional: log the errors)
            foreach (var error in createUserResult.Errors)
            {
                Console.WriteLine($"Error creating user: {error.Description}");
            }
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Create and assign role to the "HR" user
    string email = "david@HR.com";
    string password = "Dav9000@";

    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        user = new IdentityUser { UserName = email, Email = email };

        // Create the user
        var createUserResult = await userManager.CreateAsync(user, password);

        if (createUserResult.Succeeded)
        {
            // If user creation is successful, add the user to the "HR" role
            await userManager.AddToRoleAsync(user, "HR");
        }
        else
        {
            // Handle user creation failure (optional: log the errors)
            foreach (var error in createUserResult.Errors)
            {
                Console.WriteLine($"Error creating user: {error.Description}");
            }
        }
    }
}



app.Run();
