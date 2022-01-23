using ChatApp.Entities;

namespace ChatApp.DTOs;

public class RoomsFeedDto
{
    public string RoomTopicName { get; set; }
    public string RoomName { get; set; }
    public string HostName { get; set; }
    public string AvatarUrl { get; set; }
    public int NoParticipants { get; set; }
    public string Created { get; set; }
    public string Updated { get; set; }
    public string Description { get; set; }
}