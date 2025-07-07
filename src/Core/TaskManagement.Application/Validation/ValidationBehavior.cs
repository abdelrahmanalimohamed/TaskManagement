namespace TaskManagement.Application.Validation;
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
 where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;
	private readonly IServiceProvider _serviceProvider;
	public ValidationBehavior(
		IEnumerable<IValidator<TRequest>> validators, 
		IServiceProvider serviceProvider)
	{
		_validators = validators;
		_serviceProvider = serviceProvider;
	}
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var failures = new List<ValidationFailure>();

		foreach (var validator in _validators)
		{
			var result = await validator.ValidateAsync(request, cancellationToken);
			if (!result.IsValid)
				failures.AddRange(result.Errors);
		}

		var properties = request.GetType().GetProperties();
		foreach (var property in properties)
		{
			var value = property.GetValue(request);
			if (value == null) continue;

			var validatorType = typeof(IValidator<>).MakeGenericType(value.GetType());
			var validator = _serviceProvider.GetService(validatorType) as IValidator;
			if (validator == null) continue;

			var validationContext = new ValidationContext<object>(value);
			var result = await validator.ValidateAsync(validationContext, cancellationToken);
			if (!result.IsValid)
				failures.AddRange(result.Errors);
		}
		if (failures.Any())
		{
			var failureDictionary = failures
				.GroupBy(f => f.PropertyName)
				.ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());

			throw new ValidationsException(failureDictionary);
		}
		return await next();
	}
}