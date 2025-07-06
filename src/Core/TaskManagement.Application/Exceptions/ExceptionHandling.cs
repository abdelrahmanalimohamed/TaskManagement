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

		status = exception switch
		{
			CustomDuplicateNameException => HttpStatusCode.BadRequest,
			ValidationsException => HttpStatusCode.BadRequest,
			_ => HttpStatusCode.InternalServerError
		};

		response = exception switch
		{
			CustomDuplicateNameException duplicateException => new
			{
				statusCode = (int)status,
				error = "Duplication error",
				message = duplicateException.InnerException != null
					? duplicateException.InnerException.Message
					: duplicateException.Message
			},
			ValidationsException validationException => new
			{
				statusCode = (int)status,
				error = "Validation error",
				message = validationException.Errors
			},
			_ => new
			{
				statusCode = (int)status,
				error = "InternalServerError",
				message = exception.InnerException != null
					? exception.InnerException.Message
					: exception.Message
			}
		};

		switch (exception)
		{
			case CustomDuplicateNameException duplicateException:
				_logger.LogWarning(duplicateException, "Duplicate name exception occurred.");
				break;
			case ValidationsException validationException:
				_logger.LogWarning("Validation error occurred: {Errors}", validationException.Errors);
				break;
			default:
				_logger.LogError(exception, "An unhandled exception occurred.");
				break;
		}
		context.Response.StatusCode = (int)status;
		var jsonResponse = JsonSerializer.Serialize(response);

		await context.Response.WriteAsync(jsonResponse);
	}
}