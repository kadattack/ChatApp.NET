namespace ChatApp.DTOs;

public class MessagesDto
{
    public int Id { get; set; }
    public string UserCreated { get; set; }
    public string MessageIsInRoom { get; set; }
    public string Body { get; set; }
    public string Created { get; set; }
    public string Updated { get; set; }
    public string TopicName { get; set; }

    public string AvatarUrl { get; set; }
}