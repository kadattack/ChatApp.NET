namespace ChatApp.Entities;

public class AppTopics
{
    public int Id { get; set; }
    public string TopicName { get; set; }
    public ICollection<AppRooms> AppRooms { get; set; } = new List<AppRooms>();
    public DateTime Created { get; set; } = DateTime.Now;
}