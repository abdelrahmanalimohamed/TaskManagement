namespace TaskManagement.Infrastructure.Configuration;
internal class TasksConfiguration : IEntityTypeConfiguration<Tasks>
{
	public void Configure(EntityTypeBuilder<Tasks> builder)
	{
		builder.HasKey(k => k.Id);

		builder.HasIndex(t => t.Title)
			   .IsUnique();

		builder.Property(e => e.Title)
			   .IsRequired()
			   .HasMaxLength(200);

		builder.HasOne(t => t.Users)
			   .WithMany(u => u.Tasks)
			   .HasForeignKey(t => t.UserId)
			   .IsRequired(false)
			   .OnDelete(DeleteBehavior.SetNull);
	}
}