using ECommerceApi.Application.Models.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class LoginResponse
	{
		public string AccessToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime ExpiresAt { get; set; }
	}

	public class LoginResponse1Example : IExamplesProvider<LoginResponse>
	{
		public LoginResponse GetExamples()
		{
			return new LoginResponse
			{
				AccessToken = "eyJhbGciOiJIUz9999999999",
				RefreshToken = "refresh-token-xyz",
				ExpiresAt = DateTime.UtcNow.AddHours(1)
			};
		}
	}

	public class LoginResponseExample : IExamplesProvider<SuccessDataResult<LoginResponse>>
	{
		public SuccessDataResult<LoginResponse> GetExamples()
		{
			return new SuccessDataResult<LoginResponse>(new LoginResponse
			{
				AccessToken = "eyJhbGciOiJIUz9999999999",
				RefreshToken = "refresh-token-xyz",
				ExpiresAt = DateTime.UtcNow.AddHours(1)
			});
		}
	}
}
