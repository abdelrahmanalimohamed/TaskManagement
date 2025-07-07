namespace TaskManagement.UnitTests.Domain;
public class TasksTests
{
	[Fact]
	public void Can_Create_Tasks_With_Default_Values()
	{
		var task = new Tasks();

		Assert.NotEqual(Guid.Empty, task.Id);
		Assert.NotNull(task.AssignmentHistory);
		Assert.Empty(task.AssignmentHistory);
		Assert.Null(task.UserId);
		Assert.Null(task.Users);
		Assert.Null(task.LastAssignedAt);
	}

	[Fact]
	public void Can_Set_And_Get_Title()
	{
		var task = new Tasks { Title = "Test Task" };

		Assert.Equal("Test Task", task.Title);
	}

	[Fact]
	public void Can_Set_And_Get_State()
	{
		var task = new Tasks { State = TaskState.Waiting };

		Assert.Equal(TaskState.Waiting, task.State);
	}

	[Fact]
	public void Can_Set_And_Get_UserId()
	{
		var guid = Guid.NewGuid();
		var task = new Tasks { UserId = guid };

		Assert.Equal(guid, task.UserId);
	}

	[Fact]
	public void Can_Set_And_Get_Users()
	{
		var user = new Users { Name = "John Doe" };
		var task = new Tasks { Users = user };

		Assert.Equal(user, task.Users);
	}

	[Fact]
	public void Can_Set_And_Get_LastAssignedAt()
	{
		var now = DateTime.UtcNow;
		var task = new Tasks { LastAssignedAt = now };

		Assert.Equal(now, task.LastAssignedAt);
	}

	[Fact]
	public void AssignmentHistory_Is_Initially_Empty_Collection()
	{
		var task = new Tasks();

		Assert.NotNull(task.AssignmentHistory);
		Assert.Empty(task.AssignmentHistory);
	}

	[Fact]
	public void Can_Add_To_AssignmentHistory()
	{
		var task = new Tasks();
		var history = new TaskAssignmentHistory();
		task.AssignmentHistory.Add(history);

		Assert.Single(task.AssignmentHistory);
		Assert.Contains(history, task.AssignmentHistory);
	}
}

