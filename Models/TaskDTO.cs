namespace TaskManagementAPI.Models
{
    public class TaskDTO
    {
        public int? Id { get; set; } 
        public int UserId { get; set; }  
        public string Title { get; set; } = string.Empty;  
        public string Content { get; set; } = string.Empty;  
        public StatusMessage Status { get; set; }
        public List<int> Categories { get; set; } = new(); 
         public enum StatusMessage {
            Beklemede = 1 ,
            DevamEdiyor = 2,
            Tamamlandi = 3,
            Reddedildi = 4

        }
    }
}
