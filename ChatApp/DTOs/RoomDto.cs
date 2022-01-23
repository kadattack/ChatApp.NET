using ChatApp.Entities;

namespace ChatApp.DTOs;

public class RoomDto
{
    public int Id { get; set; }
    public string RoomName { get; set; }
    public string RoomTopic { get; set; }
    public string HostName { get; set; }
    public string Description { get; set; }

    public string AvatarUrl { get; set; }
    public ICollection<UserDto>? Participants { get; set; } = new List<UserDto>();
    public ICollection<MessagesDto>? Messages { get; set; } = new List<MessagesDto>();

    public string Updated { get; set; }
}