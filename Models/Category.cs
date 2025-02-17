namespace TaskManagementAPI.Models
{
    public class Category
    {
        public Category()
        {
            Tasks = new HashSet<Task>();
        }
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<Task> Tasks { get; set; }
    }
}