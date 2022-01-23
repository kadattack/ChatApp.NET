namespace ChatApp.DTOs;

public class ProfileDto
{
    public string UserName { get; set; }
    public string AvatarUrl { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
    public string Created { get; set; }
    public string Updated { get; set; }
    public ICollection<MessagesDto> MessagesDtos { get; set; } = new List<MessagesDto>();
    public ICollection<RoomsFeedDto> RoomDtos { get; set; } = new List<RoomsFeedDto>();
}