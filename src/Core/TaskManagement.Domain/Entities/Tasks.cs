namespace TaskManagement.Domain.Entities;
public class Tasks : BaseEntity
{
	public string Title { get; set; }
	public TaskState State { get; set; }
	public Guid? UserId { get; set; }
	public Users? Users { get; set; }
	public DateTime? LastAssignedAt { get; set; }
	public ICollection<TaskAssignmentHistory> AssignmentHistory { get; set; } = new List<TaskAssignmentHistory>();
	public bool HasBeenAssignedToAllUsers(IEnumerable<Guid> allUserIds)
	{
		var assignedUserIds = AssignmentHistory.Select(h => h.UserId).Distinct().ToList();
		return assignedUserIds.Count >= allUserIds.Count() &&
			   allUserIds.All(id => assignedUserIds.Contains(id));
	}
	public void AssignToUser(Guid userId)
	{
		UserId = userId;
		State = TaskState.InProgress;
		LastAssignedAt = DateTime.UtcNow;
	}
	public void MarkAsCompleted()
	{
		UserId = null;
		State = TaskState.Completed;
		LastAssignedAt = DateTime.Now;
	}
	public void SetToWaiting()
	{
		UserId = null;
		State = TaskState.Waiting;
		LastAssignedAt = DateTime.UtcNow;
	}

	public Guid? GetPreviousAssignedUserId()
	{
		if (AssignmentHistory.Count < 2) return null;

		var sortedHistory = AssignmentHistory
			.OrderByDescending(h => h.CreatedDate)
			.ToList();

		if (sortedHistory.Count < 2) return null;

		return sortedHistory.Skip(1).First().UserId;
	}

	public IEnumerable<Users> GetEligibleUsers(IEnumerable<Users> allUsers)
	{
		var currentUserId = UserId;
		var previousUserId = GetPreviousAssignedUserId();

		return allUsers.Where(user =>
			user.Id != currentUserId &&
			user.Id != previousUserId);
	}
	public Users? SelectRandomUser(IEnumerable<Users> eligibleUsers)
	{
		if (!eligibleUsers.Any()) return null;

		var usersList = eligibleUsers.ToList();
		var random = new Random();
		return usersList[random.Next(usersList.Count)];
	}
	public TaskAssignmentHistory CreateAssignmentHistory(Users user)
	{
		return new TaskAssignmentHistory
		{
			TaskId = Id,
			UserId = user.Id
		};
	}
}