using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using ECommerceApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers
{

	[ApiController]
	[Route("api/products")]
	public class ProductController : BaseController
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(SuccessDataResult<List<ProductResponse>>))]
		public async Task<IActionResult> GetAllProductsAsync()
		{
			var result = await _productService.GetAllProductsAsync();
			return Result(result);
		}
	}

}
