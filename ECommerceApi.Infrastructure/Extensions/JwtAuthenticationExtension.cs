using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ECommerceApi.Infrastructure.Extensions
{
	public static class JwtAuthenticationExtension
	{
		public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			var jwtSettingsSection = configuration.GetSection("Jwt");
			services.Configure<JwtSettings>(jwtSettingsSection);

			var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
			var key = Encoding.UTF8.GetBytes(jwtSettings!.Key);

			_ = services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
					ValidIssuer = jwtSettings.Issuer,
					ValidAudience = jwtSettings.Audience,
					ClockSkew = TimeSpan.Zero
				};
				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{

						var error = context.Exception switch
						{
							SecurityTokenExpiredException => "Token expired",
							SecurityTokenInvalidSignatureException => "Invalid token signature",
							_ => "Authentication failed"
						};

						context.Response.StatusCode = StatusCodes.Status401Unauthorized;
						context.Response.ContentType = "application/json";
						context.Response.Headers.Append("Token-Expired", "true");

						var response = new ErrorDataResult<object>(HttpStatusCode.Unauthorized, error);

						var json = JsonSerializer.Serialize(response);
						return context.Response.WriteAsync(JsonSerializer.Serialize(response));
					},
					OnChallenge = context =>
					{

						if (!context.Response.HasStarted)
						{
							context.HandleResponse();
							context.Response.StatusCode = StatusCodes.Status401Unauthorized;
							context.Response.ContentType = "application/json";

							var response = new ErrorDataResult<object>(HttpStatusCode.Unauthorized, "Unauthorized access");

							return context.Response.WriteAsync(JsonSerializer.Serialize(response));
						}

						return Task.CompletedTask;
					},
					OnTokenValidated = async context =>
					{
						var userIdStr = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

						if (!Guid.TryParse(userIdStr, out var userId))
						{
							context.Fail("Invalid User ID.");
							return;
						}

						if (!context.Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
						{
							context.Fail("Missing Authorization header.");
							return;
						}

						var accessToken = authHeaderValue.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

						var tokenValidator = context.HttpContext.RequestServices.GetRequiredService<IRefreshTokenService>();

						var isValid = await tokenValidator.ValidateAccessTokenAsync(userId, accessToken);

						if (!isValid)
						{
							context.Fail("Access token is invalid or revoked.");
						}
					}
				};
			});

			return services;
		}
	}
}
