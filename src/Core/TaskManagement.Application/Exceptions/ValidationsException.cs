namespace TaskManagement.Application.Exceptions;
public class ValidationsException : Exception
{
	public IDictionary<string, string[]> Errors { get; }

	public ValidationsException(IDictionary<string, string[]> errors)
		: base("Validation failed")
	{
		Errors = errors;
	}
}