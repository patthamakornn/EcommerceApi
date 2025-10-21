using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class LoginRequest
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}

	public class LoginRequestExample : IExamplesProvider<LoginRequest>
	{
		public LoginRequest GetExamples()
		{
			return new LoginRequest
			{
				Email = "test.user@example.com",
				Password = "P@sswOrd123"
			};
		}
	}
}
