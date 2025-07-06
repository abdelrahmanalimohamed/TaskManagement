namespace TaskManagement.Application.Interfaces;
public interface ITaskReassignmentService
{
	Task ReassignTasksAsync(CancellationToken cancellationToken = default);
}