namespace TaskManagement.UnitTests.Infrastructure;
public class UnitOfWorkTests
{

	[Fact]
	public async Task CommitAsync_CallsSaveChangesAsync_ReturnsResult()
	{
		// Arrange
		var mockDbContext = new Mock<DbContext>();
		mockDbContext
			.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(5);

		var unitOfWork = new UnitOfWork(mockDbContext.Object);

		// Act
		var result = await unitOfWork.CommitAsync();

		// Assert
		Assert.Equal(5, result);
		mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public void Dispose_CallsDbContextDispose()
	{
		// Arrange
		var mockDbContext = new Mock<DbContext>();
		mockDbContext.Setup(db => db.Dispose());

		var unitOfWork = new UnitOfWork(mockDbContext.Object);

		// Act
		unitOfWork.Dispose();

		// Assert
		mockDbContext.Verify(db => db.Dispose(), Times.Once);
	}
}
