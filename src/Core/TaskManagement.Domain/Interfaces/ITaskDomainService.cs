namespace TaskManagement.Domain.Interfaces;
public interface ITaskDomainService
{
	bool HasBeenAssignedToAllUsers(Tasks task, IEnumerable<Guid> allUserIds);
	void AssignToUser(Tasks task, Guid userId);
	void MarkAsCompleted(Tasks task);
	void SetToWaiting(Tasks task);
	IEnumerable<Users> GetEligibleUsers(Tasks task, IEnumerable<Users> allUsers);
	Users? SelectRandomUser(IEnumerable<Users> eligibleUsers);
	TaskAssignmentHistory CreateAssignmentHistory(Tasks task, Users user);
}