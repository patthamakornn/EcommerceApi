using ECommerceApi.API.Extensions;
using ECommerceApi.API.Filters;
using ECommerceApi.Application;
using ECommerceApi.Infrastructure;
using ECommerceApi.Infrastructure.Persistence;
using ECommerceApi.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseFileLogging("EcommerceApi-.log");

builder.Services.AddCustomSwagger();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers(options =>
{
	options.Filters.Add<ValidateModelAttribute>();
})
.ConfigureApiBehaviorOptions(options =>
{
	options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var dbContext = services.GetRequiredService<ApplicationDbContext>();

	await DbSeeder.SeedAsync(dbContext);
}

app.UseSwaggerExtension();

app.UseMiddlewareRegistration();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
