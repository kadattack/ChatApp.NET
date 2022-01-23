using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatApp.Data;
using ChatApp.DTOs;
using ChatApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;


namespace ChatApp.Controllers;


public class RoomController : BaseApiController
{

    private readonly DataContext _context;

    public RoomController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomsFeedDto>>> GetRooms()
    {
        var roomList = new List<RoomsFeedDto>();
        var room = await _context.Rooms.Include(x=>x.Host).Include(x => x.Participants).Include(x => x.Topic).ToListAsync();
        room.ForEach(r =>
        {
            var dto = new RoomsFeedDto
            {
                RoomTopicName = r.Topic.TopicName,
                RoomName = r.RoomName,
                HostName = r.Host.UserName,
                NoParticipants = r.Participants.Count,
                Created = r.Created.ToShortDateString(),
                Updated = r.Updated.ToShortDateString(),
                Description = r.Description,
                AvatarUrl = r.Host.AvatarUrl
            };
            roomList.Add(dto);
        });
        return roomList;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RoomDto>> GetRoomByString(CreateRoomDto createRoomDto)
    {
        var room2 = await _context.Rooms.Include(x => x.Participants).Include(x => x.Host).ToListAsync();

        var room = await _context.Rooms.Include(x => x.Participants).Include(x => x.Host).Include(x=>x.AppMessages)
            .ThenInclude(x=>x.AppUsers).Include(x=>x.Topic).Where(x => x.RoomName == createRoomDto.RoomName).SingleOrDefaultAsync(x=>x.Topic.TopicName == createRoomDto.TopicName);
        if (room == null) return Unauthorized("Invalid room name.");

        var messagesList = new List<MessagesDto>();
        var particiapntsList = new List<UserDto>();

        for (var i = 0; i < room.AppMessages.Count; i++)
        {
            var m = new MessagesDto
            {
                Id = room.AppMessages.ElementAt(i).Id,
                TopicName = room.Topic.TopicName,
                UserCreated = room.AppMessages.ElementAt(i).AppUsers.UserName,
                MessageIsInRoom = room.RoomName,
                AvatarUrl = room.AppMessages.ElementAt(i).AppUsers.AvatarUrl,
                Body = room.AppMessages.ElementAt(i).Body,
                Created = room.AppMessages.ElementAt(i).Created.ToShortTimeString(),
                Updated = room.AppMessages.ElementAt(i).Updated.ToShortTimeString()
            };
            messagesList.Add(m);
        }

        for (var i = 0; i < room.Participants.Count; i++)
        {
            particiapntsList.Add(new UserDto
            {
                UserName = room.Participants.ElementAt(i).UserName,
                AvatarUrl = room.Participants.ElementAt(i).AvatarUrl
            });
        }


        var roomDto = new RoomDto
        {
            Id = room.Id,
            RoomName = room.RoomName,
            Participants = particiapntsList,
            AvatarUrl = room.Host.AvatarUrl,
            Description = room.Description,
            RoomTopic = room.Topic.TopicName,
            HostName = room.Host.UserName,
            Messages = messagesList,
            Updated = room.Updated.ToShortDateString()
        };

        return roomDto;
    }


    // [Authorize(Roles = "Admin")]
    [Authorize]
    [HttpGet("delete/{id}")]
    public async Task<ActionResult<RoomDto>> DeleteRoom(int id)
    {
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userName == null) return Unauthorized("Unauthorized Login!");
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
        if (user == null) return Unauthorized("Incorrect Login!");

        var room = await _context.Rooms.Include(x=>x.Topic).SingleOrDefaultAsync(x=>x.Id==id);
        if (room == null) return Unauthorized("No such room exists!");

        var topic = await _context.Topics.FindAsync(room.Topic.Id);
        if (topic.AppRooms.Count == 1)
        {
            _context.Topics.Remove(topic);
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        return NoContent();

    }


    [Authorize]
    [HttpGet("id/{id}")]
    public async Task<ActionResult<CreateRoomDto>> GetRoomById(int id)
    {
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userName == null) return Unauthorized("Unauthorized Login!");
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
        if (user == null) return Unauthorized("Incorrect Login!");

        var room = await _context.Rooms.Include(x=>x.Topic).SingleOrDefaultAsync(x=>x.Id == id);

        var roomDto = new CreateRoomDto
        {
            RoomName = room.RoomName,
            TopicName = room.Topic.TopicName,
            Description = room.Description
        };
        return roomDto;
    }



    [Authorize]
    [HttpPut("edit")]
    public async Task<ActionResult<RoomDto>> EditRoom(EditRoomDto editRoomDto)
    {
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userName == null) return Unauthorized("Unauthorized Login!");
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
        if (user == null) return Unauthorized("Incorrect Login!");

        var room = await _context.Rooms.Include(x=>x.Topic).SingleOrDefaultAsync(x=>x.Id == editRoomDto.Id);
        if (room == null) return Unauthorized("No such room exists!");

        var topic = await _context.Topics.FirstOrDefaultAsync(x => x.TopicName == editRoomDto.TopicName);
        if (topic == null)
        {
            var newTopic = new AppTopics
            {
                TopicName = editRoomDto.TopicName,
                AppRooms = new List<AppRooms> {room},
            };
            topic = newTopic;
            var deleteTopic = room.Topic;
            var topicToFix = await _context.Topics.FindAsync(deleteTopic.Id);
            topicToFix!.AppRooms.Remove(room);
            _context.Topics.Add(topic);
            _context.Entry(topicToFix).State = EntityState.Modified;
        }

        room.Updated = DateTime.Now;
        room.Description = editRoomDto.Description;
        room.RoomName = editRoomDto.RoomName;
        room.Topic = topic;

        _context.Entry(room).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();

    }



    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RoomDto>> CreateRoom(CreateRoomDto createRoomDto)
    {

        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userName == null) return Unauthorized("Unauthorized Login!");
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
        if (user == null) return Unauthorized("Incorrect Login!");

        var room = await _context.Rooms.Include(x => x.Topic).Where(x => x.RoomName == createRoomDto.RoomName)
            .SingleOrDefaultAsync(x => x.Topic.TopicName == createRoomDto.TopicName);
        if (room != null) return Unauthorized("Room in Topic already exists!");

        var topic = await _context.Topics.SingleOrDefaultAsync(x => x.TopicName == createRoomDto.TopicName);
        var topicWasNull = false;
        if (topic == null)
        {
            topicWasNull = true;
            topic = new AppTopics
            {
                TopicName = createRoomDto.TopicName,
                AppRooms = new List<AppRooms>(),
            };
        }



        AppRooms newRoom = new AppRooms
        {
            RoomName = createRoomDto.RoomName,
            Topic = topic,
            AppMessages = new List<AppMessages>(),
            Description = createRoomDto.Description,
            Participants = new List<AppUsers>(),
            Host = user,
            HostId = user.Id
        };

        user.HostOfRooms.Add(newRoom);
        topic.AppRooms.Add(newRoom);


        if (topicWasNull)
        {
            _context.Topics.Add(topic);
        }
        else
        {
            _context.Entry(topic).State = EntityState.Modified;
        }

        _context.Entry(user).State = EntityState.Modified;


        await _context.SaveChangesAsync();

        return NoContent();

    }



    [HttpGet("topic/{topic}")]
    public async Task<ActionResult<List<RoomsFeedDto>>> GetRoomByTopic(string topic)
    {
        if (topic == "null")
        {
            return await this.GetRooms();
        }
        var rooms = await _context.Rooms.Include(x=>x.Topic).Include(x=>x.Participants).Include(x=>x.Host).Where(x => x.Topic.TopicName == topic).ToListAsync();

        List<RoomsFeedDto> roomDtos = new List<RoomsFeedDto>();
        foreach (var appRooms in rooms)
        {
            roomDtos.Add(new RoomsFeedDto
            {
                RoomTopicName = appRooms.Topic.TopicName,
                NoParticipants = appRooms.Participants.Count,
                Created = appRooms.Created.ToShortDateString(),
                Updated = appRooms.Created.ToShortDateString(),
                RoomName = appRooms.RoomName,
                HostName = appRooms.Host.UserName,
                Description = appRooms.Description,
                AvatarUrl = appRooms.Host.AvatarUrl,
            });
        }

        return roomDtos;
    }





}