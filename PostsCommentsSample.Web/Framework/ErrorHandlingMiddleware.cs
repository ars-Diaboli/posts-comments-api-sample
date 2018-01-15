using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using NLog;

namespace PostsCommentsSample.Web.Framework
{
	public class ErrorHandlingMiddleware
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly RequestDelegate next;

		public ErrorHandlingMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = HttpStatusCode.InternalServerError;

			if (exception is ArgumentNullException) code = HttpStatusCode.BadRequest;
			//else if (exception is MyException) code = HttpStatusCode.BadRequest;

			var result = JsonConvert.SerializeObject(new { error = exception.Message });
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;
			return context.Response.WriteAsync(result);
		}
	}
}
