namespace TaskManagement.Application.Extensions;
public static class StringExtensions
{
	public static string ToNormalizedLower(this string input)
			=> input?.Trim().ToLowerInvariant() ?? string.Empty;
}