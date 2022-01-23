namespace ChatApp.Entities;

public class AppMessages
{
    public int Id { get; set; }

    public int AppUsersId { get; set; }
    public AppUsers AppUsers { get; set; }

    public int AppRoomsId { get; set; }
    public AppRooms AppRooms { get; set; }
    public string Body { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;
}