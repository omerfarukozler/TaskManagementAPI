namespace TaskManagementAPI.Models
{
    public class TaskDTO
    {
        public int? Id { get; set; } 
        public int UserId { get; set; }  
        public string Title { get; set; } = string.Empty;  
        public string Content { get; set; } = string.Empty;  
        public int Status { get; set; }
        public List<int> Categories { get; set; } = new(); 
    }
}
