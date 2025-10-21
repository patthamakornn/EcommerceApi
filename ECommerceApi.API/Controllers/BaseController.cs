using ECommerceApi.Application.Models.Common;
using Microsoft.AspNetCore.Mvc;


namespace ECommerceApi.API.Controllers
{
	[ApiController]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResult<string>))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult<string>))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(InternalServerErrorResult<string>))]
	public abstract class BaseController : ControllerBase
	{
		protected IActionResult Result(ICustomResult result)
		{
			return ApiResult.Create(result);
		}
	}
}
