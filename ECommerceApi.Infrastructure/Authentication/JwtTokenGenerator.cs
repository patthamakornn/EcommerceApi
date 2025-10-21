using ECommerceApi.Application.Interfaces.Authentication;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceApi.Infrastructure.Authentication
{
	public class JwtTokenGenerator : IJwtTokenGenerator
	{
		private readonly JwtSettings _settings;

		public JwtTokenGenerator(IOptions<JwtSettings> options)
		{
			_settings = options.Value;
		}

		public string GenerateToken(User user)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email)
			};

			var token = new JwtSecurityToken(
				issuer: _settings.Issuer,
				audience: _settings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public DateTime GetExpiration(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);

			if (jwtToken.Claims.Any(c => c.Type == "exp"))
			{
				var exp = long.Parse(jwtToken.Claims.First(c => c.Type == "exp").Value);
				return DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
			}
			return default;
		}

		public bool IsTokenExpired(string token)
		{
			var expiration = GetExpiration(token);

			if (expiration == default)
				return true;

			return expiration <= DateTime.UtcNow;
		}

		public string GenerateRefreshToken()
		{
			var randomBytes = new byte[64];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}
	}
}
