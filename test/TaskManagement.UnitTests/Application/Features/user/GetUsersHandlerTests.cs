namespace TaskManagement.UnitTests.Application.Features.user
{
	public class GetUsersHandlerTests
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly GetUsersHandler _handler;
		private readonly Mock<ILogger<GetUsersHandler>> _loggerMock;
		public GetUsersHandlerTests()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<GetUsersHandler>>();
			_handler = new GetUsersHandler(
				_userRepositoryMock.Object, 
				_mapperMock.Object, 
				_loggerMock.Object);
		}

		[Fact]
		public async Task Handle_ShouldReturnPagedUsersDTO()
		{
			// Arrange
			var requestParams = new RequestParameters { PageNumber = 1, PageSize = 10 };

			var users = new List<Users>
		{
			new Users { Name = "Alice" },
			new Users { Name = "Bob" }
		};

			var pagedUsers = new PagedList<Users>(users, 2, 1, 10);

			_userRepositoryMock
			.Setup(r => r.GetAllWithPagingAsync(
				It.Is<RequestParameters>(p => p == requestParams),
				It.IsAny<CancellationToken>()
			))
			.ReturnsAsync(pagedUsers);

			var mappedDTOs = new List<GetUsersDTO>
		{
			new GetUsersDTO(users[0].Id, users[0].Name, users[0].CreatedDate.ToString("yyyy-MM-dd")),
			new GetUsersDTO(users[1].Id, users[1].Name, users[1].CreatedDate.ToString("yyyy-MM-dd"))
		};

			_mapperMock
				.Setup(m => m.Map<IEnumerable<GetUsersDTO>>(pagedUsers))
				.Returns(mappedDTOs);

			var query = new GetUsersQuery { Parameters = requestParams };

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			_loggerMock.Verify(
				log => log.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retrieved") && v.ToString().Contains("users")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
			Assert.NotNull(result);
			Assert.Equal(2, result.Items.Count());
			Assert.Equal(mappedDTOs, result.Items);
			Assert.Equal(pagedUsers.MetaData.TotalCount, result.MetaData.TotalCount);
		}
	}
}
