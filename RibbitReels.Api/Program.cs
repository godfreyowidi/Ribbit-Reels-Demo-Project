using RibbitReels.Data;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register GraphQL (we'll wire up queries later)
builder.Services
    .AddGraphQLServer()
    .RegisterDbContextFactory<AppDbContext>()
    .AddQueryType(d => d.Name("Query")); // empty for now

var app = builder.Build();

app.MapGraphQL(); // /graphql endpoint

app.Run();
