using ChatApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers;

public class ImageController : BaseApiController
{
    private readonly DataContext _context;

    public ImageController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("{url}")]
    public async Task<ActionResult<byte[]>> GetImage(string url)
    {
        var data = await _context.ImageObjects.SingleOrDefaultAsync(x => x.Url == url);
        if (data == null) return Unauthorized("No image with that url.");
        var dataTypeAndBase64 =  data.ImageData.Split(";");
        var dataType = dataTypeAndBase64[0].Split(":")[1];
        var baser64 = dataTypeAndBase64[1].Split(",")[1];
        Byte[] b = Convert.FromBase64String(baser64);

        return File(b, dataType);

    }


}