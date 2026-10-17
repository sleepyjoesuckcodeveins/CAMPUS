using Auth0.AspNetCore.Authentication;
using CAMPUS.BLL;
using CAMPUS.DAL;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    options.OpenIdConnectEvents = new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            var principal = context.Principal!;
            var authId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var email = principal.FindFirstValue(ClaimTypes.Email) ?? "";
            var name = principal.FindFirstValue("name") ?? email;

            var service = context.HttpContext.RequestServices.GetRequiredService<UserService>();
            // Get or create the user in the database based on the Auth0 ID
            var user = await service.GetOrCreateAsync(authId, email, name);

            var identity = (ClaimsIdentity)principal.Identity!;
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
        },
        OnRemoteFailure = context =>
        {
            context.HandleResponse();
            context.Response.Redirect("/Error");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserDal>();


builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("CampusDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'CampusDb' is missing.");
}

builder.Services.AddDbContext<CampusDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
