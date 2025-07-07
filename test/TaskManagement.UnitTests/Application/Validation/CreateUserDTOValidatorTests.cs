namespace TaskManagement.UnitTests.Application.Validation;
public class CreateUserDTOValidatorTests
{
	private readonly CreateUserDTOValidator _validator;
	public CreateUserDTOValidatorTests()
	{
		_validator = new CreateUserDTOValidator();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("    ")]
	public void Should_Have_Error_When_Name_Is_Empty(string invalidName)
	{
		var model = new CreateUserDTO(invalidName);
		var result = _validator.TestValidate(model);

		result.ShouldHaveValidationErrorFor(x => x.name)
			  .WithErrorMessage("The user name is required.");
	}

	[Fact]
	public void Should_Not_Have_Error_When_Name_Is_Valid()
	{
		var model = new CreateUserDTO ("Test");

		var result = _validator.TestValidate(model);

		result.ShouldNotHaveValidationErrorFor(x => x.name);
	}
}
