using ApsWebApp;
using ApsWebApp.Areas.Identity;
using ApsWebApp.Data;
using ApsWebApp.Models;
using ApsWebApp.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using ShareModels;
using System.Security.Authentication;
using System.Text.Json.Serialization;
using ApsWebApp.Validations;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsProduction())
{
    builder.WebHost.UseKestrel(serverOptions =>
    {
        serverOptions.ListenLocalhost(5000);
    });


    builder.WebHost.UseIISIntegration().UseKestrel(kestrelOptions =>
    {
        kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.SslProtocols = SslProtocols.Tls12;
        });
    }).UseIIS();
}


// Add services to the container.
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.Add(new ServiceDescriptor(
                    typeof(IExceptionNotificationService),
                    typeof(ExceptionNotificationService),
                    ServiceLifetime.Singleton));

builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen();
builder.Services.AddMyServices();
builder.Services.AddValidatorServices();
builder.Services.AddFastReport();


builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.Preserve);
//.AddNewtonsoftJson(options =>
// options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
// );

builder.Services.AddRadzenComponents();

builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "MyApplicationTheme"; // The name of the cookie
    options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
});


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.EnsureCreated())
            context.Database.Migrate();
        var userManager = services.GetRequiredService<IUserService>();
        await DbInitializer.Initialize(context, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();
app.UseFastReport();
app.UseDefaultFiles();

//app.MapControllers();
//app.MapBlazorHub();
//app.MapFallbackToPage("/_Host");
app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
    endpoints.MapHub<ElishAppHub>("/elishapp");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id}");
});



app.Run();
