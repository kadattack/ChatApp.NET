using ChatApp.Data;
using ChatApp.DTOs;
using ChatApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers;

public class TopicsController : BaseApiController
{
    private readonly DataContext _context;

    public TopicsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<BrowseTopicsDto>>> GetBrowseTopics()
    {
        var topicList = new List<BrowseTopicsDto>();
        var topics = await _context.Topics.Include("AppRooms.AppMessages").ToListAsync();
        topics.ForEach(topic =>
        {

            var noMessages = 0;
            foreach (var topicAppRoom in topic.AppRooms)
            {
                Console.WriteLine();
                Console.WriteLine(topicAppRoom);
                Console.WriteLine();

                noMessages += topicAppRoom.AppMessages.Count;
            }

            topicList.Add( new BrowseTopicsDto
            {
                TopicName = topic.TopicName,
                MessagesNo = noMessages
            });
        });

        return topicList;
    }
}