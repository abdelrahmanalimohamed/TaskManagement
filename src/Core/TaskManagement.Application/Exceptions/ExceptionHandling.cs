namespace TaskManagement.Application.Exceptions;
public class ExceptionHandling
{
	private readonly RequestDelegate _next;

	private readonly ILogger<ExceptionHandling> _logger;
	public ExceptionHandling(
		RequestDelegate next, 
		ILogger<ExceptionHandling> logger)
	{
		_next = next;
		_logger = logger;
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
			_logger.LogWarning(exception, "Duplicate name exception occurred.");
		}
		else
		{
			status = HttpStatusCode.InternalServerError;
			_logger.LogError(exception, "An unhandled exception occurred.");
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