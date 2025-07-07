namespace TaskManagement.UnitTests.Domain;
public class UsersTests
{
	[Fact]
	public void User_Initialization_ShouldSetPropertiesCorrectly()
	{
		// Arrange
		var userName = "Test User";

		// Act
		var user = new Users { Name = userName };

		// Assert
		user.Id.Should().NotBe(Guid.Empty);
		user.CreatedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
		user.Name.Should().Be(userName);
		user.Tasks.Should().BeEmpty();
		user.AssignmentHistory.Should().BeEmpty();
	}
}
