namespace TaskManagementAPI.Models
{
    public class Task
    {
        public Task()
        {
            Categories = new HashSet<Category>();
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Title { get; set; }

        public string? Content { get; set; }

        public required int Status { get; set; }
        public User? User { get; set; }

        public ICollection<Category> Categories { get; set; }

    }
}