using Microsoft.EntityFrameworkCore;
using RibbitReels.Data;
using RibbitReels.Services.Interfaces;
using RibbitReels.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register services
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ILeafService, LeafService>();

var app = builder.Build();

// Enable endpoints
app.MapControllers();

app.Run();
