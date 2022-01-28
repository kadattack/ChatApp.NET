using System.Security.Claims;
using ChatApp.Controllers;
using ChatApp.Data;
using ChatApp.DTOs;
using ChatApp.Entities;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.SignalR;

public class MessageHub : Hub
{
    private readonly DataContext _context;
    private readonly RoomController _roomController;


    public MessageHub(DataContext context)
    {
        _context = context;
        _roomController = new RoomController(context);
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var roomName = httpContext.Request.Query["roomname"].ToString();
        var topicName = httpContext.Request.Query["topicname"].ToString();

        await Groups.AddToGroupAsync(Context.ConnectionId, topicName + roomName);
        var groupRoom = new CreateRoomDto
        {
            RoomName = roomName,
            TopicName = topicName,
            Description = ""
        };
        var messages = await _roomController.GetRoomByString(groupRoom);
        await Clients.Group(topicName + roomName).SendAsync("ReceiveRoomMessage", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(MessagesDto messagesDto)
    {
        var test = messagesDto;
        var userName = Context.User.FindFirst(ClaimTypes.Name)?.Value;
        if (userName == null) throw  new HubException("Unauthorized Login!");
        var user = await _context.Users.Include(x=> x.ParticipantOfRooms).SingleOrDefaultAsync(x => x.UserName == messagesDto.UserCreated);
        if (user == null) throw new HubException("User doesn't exist");
        var room = await _context.Rooms.Include(x=>x.Participants).Include(x=>x.Host).SingleOrDefaultAsync(x => x.RoomName == messagesDto.MessageIsInRoom);
        if (room == null) throw new HubException("Room doesn't exist");
        if (userName != user.UserName) throw new HubException("You are not the owner of this message!");

        var recievedMsg = new AppMessages
        {
            AppUsersId = user.Id,
            AppUsers = user,
            AppRoomsId = room.Id,
            AppRooms = room,
            Body = messagesDto.Body,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        _context.Messages.Add(recievedMsg);
        user.AppMessages.Add(recievedMsg);
        room.AppMessages.Add(recievedMsg);


        var newPartic = room.Participants.Where(x => x.Id == user.Id);
        if (newPartic.Count() == 0)
        {
            room.Participants.Add(user);
            user.ParticipantOfRooms.Add(room);
        }

        _context.Entry(user).State = EntityState.Modified;
        _context.Entry(room).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        var msgs = _context.Messages.OrderBy(x=>x.Created).LastOrDefault();

        var newMessageDto = new MessagesDto
        {
            Id = msgs.Id,
            UserCreated = user.UserName,
            MessageIsInRoom = room.RoomName,
            Body = messagesDto.Body,
            Created = msgs.Created.ToShortTimeString(),
            Updated = msgs.Created.ToShortTimeString(),
            TopicName = messagesDto.TopicName,
            AvatarUrl = user.AvatarUrl
        };


        await Clients.Group(messagesDto.TopicName + messagesDto.MessageIsInRoom).SendAsync("NewMessage", newMessageDto);

    }
}