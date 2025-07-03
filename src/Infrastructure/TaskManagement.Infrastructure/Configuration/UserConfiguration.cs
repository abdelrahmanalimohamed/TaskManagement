namespace TaskManagement.Infrastructure.Configuration;
internal class UserConfiguration : IEntityTypeConfiguration<Users>
{
	public void Configure(EntityTypeBuilder<Users> builder)
	{
		builder.HasKey(k => k.Id);
	
		builder.HasIndex(u => u.Name)
			   .IsUnique();

		builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
	}
}