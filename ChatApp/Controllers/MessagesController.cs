using System.Security.Claims;
using ChatApp.Data;
using ChatApp.DTOs;
using ChatApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Controllers;

public class MessagesController : BaseApiController
{
    private readonly DataContext _context;

    public MessagesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<AppMessages>>> GetMessages()
    {
        return await _context.Messages.ToListAsync();
    }

    [HttpGet("{roomNo}")]
    public async Task<ActionResult<ICollection<AppMessages>>> GetMessagesFromRoom(int roomNo)
    {
        var room = _context.Rooms.Where(r => r.Id == roomNo)
            .SelectMany(r => r.AppMessages) ;
        return await room.ToListAsync();
    }


    [HttpGet("activity")]
    public async Task<ActionResult<ICollection<MessagesDto>>> GetActivityFeed()
    {
        var lastActivities = new List<MessagesDto>();
        var rooms = await _context.Messages.Include(x => x.AppRooms).ThenInclude(x=>x.Topic).Include(x => x.AppUsers).OrderByDescending(x=>x.Created).ToListAsync();

        var top = rooms.Count < 6 ? rooms.Count : 5;

        for (var i = 0; i < top; i++)
        {
            var r = rooms.ElementAt(i);
            var tlen = r.Body.Length < 50 ? r.Body.Length : 50;

            lastActivities.Add(new MessagesDto
            {
                Id = r.Id,
                TopicName = r.AppRooms.Topic.TopicName,
                UserCreated = r.AppUsers.UserName,
                AvatarUrl = r.AppUsers.AvatarUrl,
                MessageIsInRoom = r.AppRooms.RoomName,
                Body = r.Body.Substring(0,tlen),
                Created = r.Created.ToShortTimeString(),
                Updated = r.Updated.ToShortTimeString()

            });
        }
        return lastActivities;
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> PostMessage(MessagesDto messagesDto)
    {
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userName == null) return Unauthorized("Unauthorized Login!");
        var user = await _context.Users.Include(x=> x.ParticipantOfRooms).SingleOrDefaultAsync(x => x.UserName == messagesDto.UserCreated);
        if (user == null) Unauthorized("User doesn't exist");
        var room = await _context.Rooms.Include(x=>x.Participants).SingleOrDefaultAsync(x => x.RoomName == messagesDto.MessageIsInRoom);
        if (room == null) Unauthorized("Room doesn't exist");
        if (userName != user.UserName) return Unauthorized("You are not the owner of this message!");

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

        return NoContent();
    }



    [HttpPut("delete")]
    [Authorize]
    public async Task<ActionResult> DeteleMessage(MessagesDto messagesDto)
    {
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userName == null) return Unauthorized("Unauthorized Login!");
        var user = await _context.Users.Include(x=> x.ParticipantOfRooms).SingleOrDefaultAsync(x => x.UserName == messagesDto.UserCreated);
        if (user == null) Unauthorized("User doesn't exist");
        var room = await _context.Rooms.Include(x=>x.Participants).SingleOrDefaultAsync(x => x.RoomName == messagesDto.MessageIsInRoom);
        if (room == null) Unauthorized("Room doesn't exist");
        if (userName != user.UserName) return Unauthorized("You are not the owner of this message!");

        var message = await _context.Messages.FindAsync(messagesDto.Id);
        if (message == null) return Unauthorized("No such message!");

        _context.Messages.Remove(message);


        await _context.SaveChangesAsync();


        return NoContent();


    }

}