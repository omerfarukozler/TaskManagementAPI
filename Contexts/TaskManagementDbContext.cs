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

}
