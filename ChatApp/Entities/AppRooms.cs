using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Entities;

public class AppRooms
{
    public int Id { get; set; }
    public string RoomName { get; set; }

    // [ForeignKey("AppUsers")]
    public int HostId { get; set; }
    public AppUsers Host { get; set; }

    public AppTopics Topic { get; set; }

    public ICollection<AppMessages> AppMessages { get; set; } = new List<AppMessages>();
    public string Description { get; set; } = "";

    public ICollection<AppUsers> Participants { get; set; } = new List<AppUsers>();

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;

}