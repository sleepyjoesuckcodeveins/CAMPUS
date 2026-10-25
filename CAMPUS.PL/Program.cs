using CAMPUS.BLL.Interfaces;
using CAMPUS.BLL.Services;
using CAMPUS.DAL;
using CAMPUS.DAL.Repositories;
using CAMPUS.DOMAIN.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("CampusDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'CampusDb' is missing.");
}

builder.Services.AddDbContext<CampusDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

// Repositories
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

// Services
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IRequestService, RequestService>();     // Person 3
builder.Services.AddScoped<IOverviewService, OverviewService>();   // Person 4
builder.Services.AddScoped<IAssignmentService, AssignmentService>();   // Person 4

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();