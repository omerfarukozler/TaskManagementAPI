using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

public class TaskManagementDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<TaskManagementAPI.Models.Task> Tasks { get; set; }
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .Property(t => t.CreatedAtUnix)
        .HasDefaultValue((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
}


}
