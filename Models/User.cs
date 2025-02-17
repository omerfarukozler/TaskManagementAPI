using System.ComponentModel.DataAnnotations.Schema;
namespace TaskManagementAPI.Models
{
    public class User
    {
        public User()
        {
            Tasks = new HashSet<Task>();
        }
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }

        public int CreatedAtUnix { get; set; }

        public ICollection<Task> Tasks { get; set; }

        [NotMapped]
        public DateTime CreatedAt
        {
            get => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnix).UtcDateTime;
            set => CreatedAtUnix = (int)new DateTimeOffset(value).ToUnixTimeSeconds();
        }
    }
}