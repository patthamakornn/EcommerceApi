using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class RegisterRequest
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
	}

	public class RegisterRequestExample : IExamplesProvider<RegisterRequest>
	{
		public RegisterRequest GetExamples()
		{
			return new RegisterRequest
			{
				Email = "test.user@example.com",
				Password = "P@sswOrd123",
				FirstName = "A",
				LastName = "B"
			};
		}
	}
}
