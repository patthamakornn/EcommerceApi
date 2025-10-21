using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace ECommerceApi.Application.Models.Common
{
	public class ApiResult : IActionResult
	{
		private readonly ICustomResult _result;

		private ApiResult(ICustomResult result)
		{
			_result = result ?? throw new ArgumentNullException(nameof(result));
		}

		private ApiResult(object data)
		{
			_result = (ICustomResult)data;
		}

		public static IActionResult Create(ICustomResult result)
		{
			return new ApiResult(result);
		}

		public static IActionResult Create(object data)
		{
			return new ApiResult(data);
		}

		public async Task ExecuteResultAsync(ActionContext context)
		{
			object? value = _result;
			int status = StatusCodes.Status500InternalServerError;

			switch (_result)
			{
				case SuccessDataResult:
					status = StatusCodes.Status200OK;
					break;

				case CreateDataResult:
					status = StatusCodes.Status201Created;
					break;

				case Result result:
					status = result.StatusCode;
					break;

				default:
					var resultType = _result?.GetType();
					if (resultType != null && resultType.IsGenericType)
					{
						var genericTypeDef = resultType.GetGenericTypeDefinition();

						if (genericTypeDef == typeof(SuccessDataResult<>))
						{
							status = StatusCodes.Status200OK;
						}
						else if (genericTypeDef == typeof(CreateDataResult<>))
						{
							status = StatusCodes.Status201Created;
						}
					}
					break;
			}

			await new ObjectResult(value)
			{
				StatusCode = status
			}.ExecuteResultAsync(context);
		}

	}
}
