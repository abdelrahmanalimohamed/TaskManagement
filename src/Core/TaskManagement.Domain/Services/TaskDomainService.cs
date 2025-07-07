namespace TaskManagement.Domain.Services;
public class TaskDomainService : ITaskDomainService
{
	public bool HasBeenAssignedToAllUsers(Tasks task, IEnumerable<Guid> allUserIds)
	{
		var assignedUserIds = task.AssignmentHistory
								  .Select(h => h.UserId)
								  .Distinct()
								  .ToList();

		return assignedUserIds.Count >= allUserIds.Count() &&
			   allUserIds.All(id => assignedUserIds.Contains(id));
	}

	public void AssignToUser(Tasks task, Guid userId)
	{
		task.UserId = userId;
		task.State = TaskState.InProgress;
		task.LastAssignedAt = DateTime.Now;
	}

	public void MarkAsCompleted(Tasks task)
	{
		task.UserId = null;
		task.State = TaskState.Completed;
		task.LastAssignedAt = DateTime.Now;
	}

	public void SetToWaiting(Tasks task)
	{
		task.UserId = null;
		task.State = TaskState.Waiting;
		task.LastAssignedAt = DateTime.Now;
	}

	public IEnumerable<Users> GetEligibleUsers(Tasks task, IEnumerable<Users> allUsers)
	{
		var currentUserId = task.UserId;
		var previousUserId = GetPreviousAssignedUserId(task);

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

	public TaskAssignmentHistory CreateAssignmentHistory(Tasks task, Users user)
	{
		return new TaskAssignmentHistory
		{
			TaskId = task.Id,
			UserId = user.Id
		};
	}
	private Guid? GetPreviousAssignedUserId(Tasks task)
	{
		if (task.AssignmentHistory.Count < 2) return null;

		var sortedHistory = task.AssignmentHistory
			.OrderByDescending(h => h.CreatedDate)
			.ToList();

		return sortedHistory.Skip(1).FirstOrDefault()?.UserId;
	}
}
