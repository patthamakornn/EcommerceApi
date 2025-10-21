using ECommerceApi.Application.Interfaces.Services;

namespace ECommerceApi.Infrastructure.Authentication
{
	public class PasswordService : IPasswordService
	{
		public string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		public bool VerifyPassword(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}
	}
}
