namespace TaskManagement.Application.Exceptions;
public class CustomDuplicateNameException : Exception
{
	public CustomDuplicateNameException()
	{
	}

	public CustomDuplicateNameException(string message)
		: base(message)
	{
	}

	public CustomDuplicateNameException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
