using ChatApp.Data;
using ChatApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers;

public class SearchController : BaseApiController

{
    private readonly DataContext _context;
    public SearchController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("{searchroomname}")]
    public async Task<ActionResult<IEnumerable<RoomsFeedDto>>> GetRoomsBySearch(string? searchroomname)
    {

        List<Entities.AppRooms>? room = await _context.Rooms.Include(x => x.Participants).Include(x=> x.Topic).Where(x=> x.RoomName.ToLower().Contains(searchroomname.ToLower())).ToListAsync();


        var roomList = new List<RoomsFeedDto>();
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


}