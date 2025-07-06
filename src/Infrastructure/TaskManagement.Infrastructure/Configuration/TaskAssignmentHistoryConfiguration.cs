namespace TaskManagement.Infrastructure.Configuration;
public class TaskAssignmentHistoryConfiguration : IEntityTypeConfiguration<TaskAssignmentHistory>
{
	public void Configure(EntityTypeBuilder<TaskAssignmentHistory> builder)
	{
		builder.HasOne(x => x.Task)
			   .WithMany(x => x.AssignmentHistory)
			   .HasForeignKey(x => x.TaskId);

		builder.HasOne(x => x.User)
		       .WithMany(x => x.AssignmentHistory)
			   .HasForeignKey(x => x.UserId);
	}
}