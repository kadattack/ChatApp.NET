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


namespace ChatApp.Controllers;

// [Authorize]
public class UsersController : BaseApiController
{

    private readonly DataContext _context;
    public UsersController(DataContext context)
    {
        _context = context;
    }


    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AppUsers>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }


    // [HttpGet("{id}")]
    // public async Task<ActionResult<AppUsers>> GetUser(int id)
    // {
        // return await _context.Users.FindAsync(id);
    // }


    // [HttpGet("{username}")]
    // public async Task<ActionResult<AppUsers>> GetUser(string username)
    // {
    //     return await _context.Users.SingleOrDefaultAsync(x=>x.UserName == username);
    // }

    [Authorize]
    [HttpGet("{username}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(string username)
    {

        var user = await _context.Users
            .Include(x => x.AppMessages).ThenInclude(r=>r.AppRooms).ThenInclude(x=>x.Topic)
            .Include(x => x.HostOfRooms).ThenInclude(h=>h.Topic)
            .Include(x => x.HostOfRooms).ThenInclude(h=>h.Participants)
            .SingleOrDefaultAsync(j => j.UserName == username);


        if (user == null) return Unauthorized("Invalid user");
        user.AppMessages = user.AppMessages?.OrderByDescending(x => x.Created).ToList();


        // var user2 = await _context.Users.Include("AppMessages.Room").OrderByDescending(x:AppMessages=>x.Creatd).Where(x => x.UserName == "tom").ToListAsync();


        var messagesList = new List<MessagesDto>();
        var roomsList = new List<RoomsFeedDto>();

        if (user.AppMessages != null)
        {
            for (var i = 0; i < user.AppMessages.Count; i++)
            {


                var msg = new MessagesDto
                {
                    Id = user.AppMessages.ElementAt(i).Id,
                    TopicName = user.AppMessages.ElementAt(i).AppRooms.Topic.TopicName,
                    UserCreated = user.AppMessages.ElementAt(i).AppUsers.UserName,
                    MessageIsInRoom = user.AppMessages.ElementAt(i).AppRooms.RoomName,
                    Body = user.AppMessages.ElementAt(i).Body,
                    AvatarUrl = user.AppMessages.ElementAt(i).AppUsers.AvatarUrl,
                    Created = user.AppMessages.ElementAt(i).Created.ToShortDateString(),
                    Updated = user.AppMessages.ElementAt(i).Updated.ToShortDateString()
                };

                messagesList.Add(msg);
            }
        }


        if (user.HostOfRooms != null)
        {
            for (var i = 0; i < user.HostOfRooms.Count; i++)
            {

                roomsList.Add(new RoomsFeedDto
                {
                    RoomTopicName = user.HostOfRooms.ElementAt(i).Topic.TopicName,
                    RoomName = user.HostOfRooms.ElementAt(i).RoomName,
                    HostName = user.UserName,
                    AvatarUrl = user.AvatarUrl,
                    NoParticipants = user.HostOfRooms.ElementAt(i).Participants.Count,
                    Created = user.HostOfRooms.ElementAt(i).Created.ToShortDateString(),
                    Updated = user.HostOfRooms.ElementAt(i).Created.ToShortDateString(),
                    Description = user.HostOfRooms.ElementAt(i).Description
                });
            }
        }

        return new ProfileDto
        {
            UserName = user.UserName,
            AvatarUrl = user.AvatarUrl,
            Email = user.Email,
            Bio = user.Bio,
            Created = user.Created.ToString(),
            Updated = user.Updated.ToString(),
            MessagesDtos = messagesList,
            RoomDtos = roomsList
        };
    }



    [HttpPut]
    [Authorize]
    public async Task<ActionResult> UpdateProfile(ProfileUpdateDto profileUpdateDto)
    {
        // This returns the username from the token given by client
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userName == null) return Unauthorized("Unauthorized Login!");
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
        if (user == null) return Unauthorized("Incorrect Login!");

        if (profileUpdateDto.AvatarImageObject != null)
        {
            var urlArr = profileUpdateDto.AvatarImageObject.Url.Split("/");
            var url = urlArr[urlArr.Length - 1];

            var foundImage = await _context.ImageObjects.FirstOrDefaultAsync(x => x.Url == url);
            if (foundImage == null)
            {
                _context.ImageObjects.Add(new AppImageObject
                {
                    Url = url,
                    ImageData = profileUpdateDto.AvatarImageObject.ImageData
                });
            }
            user.AvatarUrl = url;
        }



        user.UserName = profileUpdateDto.UserName;
        user.Bio = profileUpdateDto.Bio;
        user.Email = profileUpdateDto.Email;

        // Save changed to database
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}