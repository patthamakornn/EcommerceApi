using ECommerceApi.Application.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ECommerceApi.API.Filters
{
	public class ValidateModelAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.ModelState.IsValid)
			{
			
				var errors = context.ModelState
				.Where(x => x.Value?.Errors.Count > 0)
				.SelectMany(kvp => kvp.Value!.Errors.Select(error => new
				{
					Message = error.ErrorMessage
				})).ToList();

				var response = new ErrorDataResult<object>(HttpStatusCode.BadRequest, errors);

				context.Result = new JsonResult(response)
				{
					StatusCode = (int)HttpStatusCode.BadRequest
				};
			}
		}
	}
}
