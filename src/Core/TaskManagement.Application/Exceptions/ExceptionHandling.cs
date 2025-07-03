namespace TaskManagement.Application.Exceptions;
public class ExceptionHandling
{
	private readonly RequestDelegate _next;
	public ExceptionHandling(RequestDelegate next)
	{
		_next = next;
	}
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(
		HttpContext context,
		Exception exception)
	{
		context.Response.ContentType = "application/json";

		HttpStatusCode status;
		object response;

		if (exception is CustomDuplicateNameException)
		{
			status = HttpStatusCode.BadRequest;
		}
		else
		{
			status = HttpStatusCode.InternalServerError;
		}

		response = new
		{
			statusCode = (int)status,
			error = status.ToString(),
			message = exception.InnerException != null
				? exception.InnerException.Message
				: exception.Message,
		};

		context.Response.StatusCode = (int)status;
		var jsonResponse = JsonSerializer.Serialize(response);

		await context.Response.WriteAsync(jsonResponse);
	}
}