using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace ECommerceApi.Application.Models.Common
{
	public class DataResult<T> : Result
	{
		public T Data { get; }
		public DataResult() { }

		public DataResult(HttpStatusCode statusCode, T data) : base(statusCode)
		{
			Data = data;
		}

		public DataResult(HttpStatusCode statusCode, string statusMessage, T data) : base(statusCode, statusMessage)
		{
			Data = data;
		}
	}
	public class Result : ICustomResult
	{
		public int StatusCode { get; }
		public string StatusMessage { get; }

		public Result(HttpStatusCode statusCode)
		{
			StatusCode = (int)statusCode;

			switch (statusCode)
			{
				case HttpStatusCode.OK:
					StatusMessage = HttpStatusCode.OK.ToString();
					break;
				case HttpStatusCode.Created:
					StatusMessage = HttpStatusCode.Created.ToString();
					break;
				case HttpStatusCode.BadRequest:
					StatusMessage = HttpStatusCode.BadRequest.ToString();
					break;
				case HttpStatusCode.Unauthorized:
					StatusMessage = HttpStatusCode.Unauthorized.ToString();
					break;
				case HttpStatusCode.NotFound:
					StatusMessage = HttpStatusCode.NotFound.ToString();
					break;
				case HttpStatusCode.InternalServerError:
					StatusMessage = HttpStatusCode.InternalServerError.ToString();
					break;
				default:
					StatusMessage = HttpStatusCode.OK.ToString();
					break;
			}
		}

		public Result(HttpStatusCode statusCode, string statusMessage)
		{
			StatusCode = (int)statusCode;
			StatusMessage = statusMessage;
		}

		public Result() { }
	}

	public interface ICustomResult
	{
		int StatusCode { get; }
		string StatusMessage { get; }
	}

	public class SuccessDataResult : Result
	{
		public SuccessDataResult() : base(HttpStatusCode.OK) { }
	}

	public class SuccessDataResult<T> : DataResult<T>
	{
		public SuccessDataResult(T data) : base(HttpStatusCode.OK, data) { }
	}

	public class CreateDataResult<T> : DataResult<T>
	{
		public CreateDataResult(T data) : base(HttpStatusCode.Created, data) { }
	}

	public class CreateDataResult : Result
	{
		public CreateDataResult() : base(HttpStatusCode.Created) { }
	}

	public class ErrorDataResult<T> : ErrorResult<T>
	{
		public ErrorDataResult()
		{
		}

		public ErrorDataResult(HttpStatusCode statusCode, T error) : base(statusCode, error) { }
	}


	public class BadRequestResult<T> : ErrorDataResult<T>
	{
		public BadRequestResult(HttpStatusCode statusCode, T data) : base(statusCode, data) { }
	}

	public class UnauthorizedResult<T> : ErrorDataResult<T>
	{
		public UnauthorizedResult(HttpStatusCode statusCode, T data) : base(statusCode, data) { }
	}

	public class InternalServerErrorResult<T> : ErrorDataResult<T>
	{
		public InternalServerErrorResult(HttpStatusCode statusCode, T data) : base(statusCode, data) { }
	}

	public class ErrorResult<T> : Result
	{
		public ErrorResult() { }

		public ErrorResult(HttpStatusCode statusCode, T error) : base(statusCode)
		{
			ResponseException = error;
		}

		public ErrorResult(HttpStatusCode statusCode) : base(statusCode) { }

		public T ResponseException { get; set; }
	}

	public sealed class ResponseException
	{
		public required string Message { get; set; }
	}

	public class ResultExample : IExamplesProvider<Result>
	{
		public Result GetExamples()
		{
			return new Result(HttpStatusCode.OK);
		}
	}
	public class BadRequestResultStringExample : IExamplesProvider<BadRequestResult<string>>
	{
		public BadRequestResult<string> GetExamples()
		{
			return new BadRequestResult<string>(HttpStatusCode.BadRequest ,"Bad request");
		}
	}

	public class UnauthorizedResultStringExample : IExamplesProvider<UnauthorizedResult<string>>
	{
		public UnauthorizedResult<string> GetExamples()
		{
			return new UnauthorizedResult<string>(HttpStatusCode.Unauthorized, "Unauthorized access");
		}
	}

	public class InternalServerErrorResultStringExample : IExamplesProvider<InternalServerErrorResult<string>>
	{
		public InternalServerErrorResult<string> GetExamples()
		{
			return new InternalServerErrorResult<string>(HttpStatusCode.InternalServerError, "Internal server error");
		}
	}
}
