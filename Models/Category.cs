using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public ICollection<Task> Tasks { get; set; }
    }
}