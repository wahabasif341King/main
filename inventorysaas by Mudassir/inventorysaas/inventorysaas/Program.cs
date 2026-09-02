using Microsoft.EntityFrameworkCore;
using InventorySaaS_Application.Infrastructure.Data;
using InventorySaaS_Application.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ICurrentUserService has no real implementation yet — that's his JWT/claims
// work. Register it once he provides it. Nothing below depends on it existing
// yet, this is just so the class list is already correct when he does.
// builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<TaxService>();
builder.Services.AddScoped<ProductVariantService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<StockTransferService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// app.UseAuthentication();  // add once his JWT scheme is configured
app.UseAuthorization();

app.MapControllers();

app.Run();