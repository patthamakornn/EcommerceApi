using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class RefreshTokenRequest
	{
		public string RefreshToken { get; set; } = string.Empty;
	}

	public class RefreshTokenRequestExample : IExamplesProvider<RefreshTokenRequest>
	{
		public RefreshTokenRequest GetExamples()
		{
			return new RefreshTokenRequest
			{
				RefreshToken = "refresh-token-xyz"
			};
		}
	}
}
