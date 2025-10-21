using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace ECommerceApi.API.Extensions
{
	public static class ApiDocumentationExtension
	{
		public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.EnableAnnotations();

				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Enter 'Bearer' followed by your JWT token"
				});

				var info = new OpenApiInfo
				{
					Title = "ECommerce API",
					Version = "v1",
					Description = $@"ECommerce API Last build date {DateTime.Now}"
				};

				options.SwaggerDoc("v1", info);
				options.ExampleFilters();

				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				options.IncludeXmlComments(xmlPath);
			});

			return services;
		}

		public static WebApplication UseSwaggerExtension(this WebApplication app)
		{
			// Configure the HTTP request pipeline.
			if (!app.Environment.IsProduction())
			{
				app.UseSwagger();

				app.UseSwaggerUI(c =>
				{
					c.RoutePrefix = "swagger";
				});
			}

			return app;
		}
	}
}
