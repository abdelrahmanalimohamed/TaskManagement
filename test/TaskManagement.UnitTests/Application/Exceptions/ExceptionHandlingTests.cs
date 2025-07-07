namespace TaskManagement.UnitTests.Application.Exceptions;
public class ExceptionHandlingTests
{
	private readonly Mock<ILogger<ExceptionHandling>> _loggerMock;
	private readonly DefaultHttpContext _context;
	public ExceptionHandlingTests()
	{
		_loggerMock = new Mock<ILogger<ExceptionHandling>>();
		_context = new DefaultHttpContext();
		_context.Response.Body = new MemoryStream();
	}

	[Fact]
	public async Task InvokeAsync_WhenNoException_CallsNext()
	{
		var wasCalled = false;
		RequestDelegate next = (_) =>
		{
			wasCalled = true;
			return Task.CompletedTask;
		};

		var middleware = new ExceptionHandling(next, _loggerMock.Object);
		await middleware.InvokeAsync(_context);

		wasCalled.Should().BeTrue();
		_context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
	}

	[Fact]
	public async Task InvokeAsync_WithCustomDuplicateNameException_ReturnsBadRequest()
	{
		RequestDelegate next = (_) => throw new CustomDuplicateNameException("Duplicate");

		var middleware = new ExceptionHandling(next, _loggerMock.Object);
		await middleware.InvokeAsync(_context);

		_context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
		_context.Response.Body.Seek(0, SeekOrigin.Begin);
		var body = await new StreamReader(_context.Response.Body).ReadToEndAsync();
		body.Should().Contain("Duplication error");
		body.Should().Contain("Duplicate");

		_loggerMock.Verify(
			l => l.Log(
				LogLevel.Warning,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Duplicate name")),
				It.IsAny<CustomDuplicateNameException>(),
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once);
	}

	[Fact]
	public async Task InvokeAsync_WithValidationsException_ReturnsBadRequestWithErrors()
	{
		var errors = new Dictionary<string, string[]>
		{
			{ "Name", new[] { "Required" } }
		};

		RequestDelegate next = (_) => throw new ValidationsException(errors);

		var middleware = new ExceptionHandling(next, _loggerMock.Object);
		await middleware.InvokeAsync(_context);

		_context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
		_context.Response.Body.Seek(0, SeekOrigin.Begin);
		var body = await new StreamReader(_context.Response.Body).ReadToEndAsync();

		var responseJson = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
		responseJson.Should().ContainKey("message");
		body.Should().Contain("Validation error");

		_loggerMock.Verify(
			l => l.Log(
				LogLevel.Warning,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Validation error")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once);
	}

	[Fact]
	public async Task InvokeAsync_WithUnhandledException_ReturnsInternalServerError()
	{
		RequestDelegate next = (_) => throw new InvalidOperationException("Unexpected");

		var middleware = new ExceptionHandling(next, _loggerMock.Object);
		await middleware.InvokeAsync(_context);

		_context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
		_context.Response.Body.Seek(0, SeekOrigin.Begin);
		var body = await new StreamReader(_context.Response.Body).ReadToEndAsync();

		body.Should().Contain("InternalServerError");
		body.Should().Contain("Unexpected");

		_loggerMock.Verify(
			l => l.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("unhandled exception")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once);
	}
}
