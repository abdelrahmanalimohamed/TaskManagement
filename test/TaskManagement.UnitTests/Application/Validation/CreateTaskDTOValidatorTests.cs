namespace TaskManagement.UnitTests.Application.Validation;
public class CreateTaskDTOValidatorTests
{
	private readonly CreateTaskDTOValidator _validator;

	public CreateTaskDTOValidatorTests()
	{
		_validator = new CreateTaskDTOValidator();
	}
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("    ")]
	public void Should_Have_Error_When_Title_Is_Empty(string invalidTitle)
	{
		var model = new CreateTaskDTO(invalidTitle);

		var result = _validator.TestValidate(model);

		result.ShouldHaveValidationErrorFor(x => x.Title)
			  .WithErrorMessage("The task name is required.");
	}
	[Fact]
	public void Should_Not_Have_Error_When_Title_Is_Valid()
	{
		var model = new CreateTaskDTO ("Important Task" );

		var result = _validator.TestValidate(model);

		result.ShouldNotHaveValidationErrorFor(x => x.Title);
	}
}