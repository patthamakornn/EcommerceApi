using ECommerceApi.Application.Models.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class RefreshTokenResponse
	{
		public string AccessToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
	}

	public class RefreshTokenResponseExample : IExamplesProvider<SuccessDataResult<RefreshTokenResponse>>
	{
		public SuccessDataResult<RefreshTokenResponse> GetExamples()
		{
			return new SuccessDataResult<RefreshTokenResponse>(new RefreshTokenResponse
			{
				AccessToken = "eyJhbGciOiJIUz999999990000",
				RefreshToken = "refresh-token-xyz",
			});
		}
	}
}
