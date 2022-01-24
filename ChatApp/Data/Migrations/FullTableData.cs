using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using ChatApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data.Migrations;

public class FullTableData
{
    public static async Task FillTables(UserManager<AppUsers> userManager, RoleManager<AppRole> roleManager, DataContext context)
    {
        if (await userManager.Users.AnyAsync()) return;

        AppMessages appMessages1 = new AppMessages
        {
            Body = "First message from Tom in room 1"
        };
        AppMessages appMessages2 = new AppMessages
        {
            Body = "Second msg from Bob in room 1"
        };
        AppMessages appMessages3 = new AppMessages
        {
            Body = "First message from Bob in room 2"
        };
        AppMessages appMessages4 = new AppMessages
        {
            Body = "Second message from Tom in room 2"
        };

        using var hmac = new HMACSHA512();

        AppUsers appUsers1 = new AppUsers
        {
            // AppRoles = new List<AppRole>(),
            UserName = "bober",
            HostOfRooms = new List<AppRooms>(),
            Email = "bob@email.com",
            Bio = "Bio of Bob",
            AppMessages = new List<AppMessages> {appMessages2, appMessages3},
            ParticipantOfRooms = new List<AppRooms>(),
            AvatarUrl = "assets/avatar.svg",
            // PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("pass")),
            // PasswordSalt = hmac.Key
        };

        AppUsers appUsers2 = new AppUsers
        {
            // AppRoles = new List<AppRole>(),
            UserName = "tomas",
            HostOfRooms = new List<AppRooms>(),
            Email = "tom@email.com",
            Bio = "Bio of Tom",
            AppMessages = new List<AppMessages> {appMessages1, appMessages4},
            ParticipantOfRooms = new List<AppRooms>(),
            AvatarUrl = "assets/avatar.svg",
            // PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("pass")),
            // PasswordSalt = hmac.Key
        };






        AppRooms appRooms1 = new AppRooms
        {
            RoomName = "Starter Questions",
            AppMessages = new List<AppMessages> {appMessages1, appMessages2},
            Description = "Chat for starters descriptiomns",
            Participants = new List<AppUsers>{appUsers1, appUsers2},
        };


        AppRooms appRooms2 = new AppRooms
        {
            RoomName = "Advanced Questions",
            AppMessages = new List<AppMessages> {appMessages3, appMessages4},
            Description = "Chat for advanced descriptiomns",
            Participants = new List<AppUsers>{appUsers1, appUsers2},
        };


        AppTopics appTopics1 = new AppTopics
        {
            TopicName = "Learning .NET",
            AppRooms = new List<AppRooms> {appRooms1, appRooms2},
        };

        appRooms1.Topic = appTopics1;
        appRooms2.Topic = appTopics1;

        appUsers1.ParticipantOfRooms = new List<AppRooms> {appRooms1, appRooms2};
        appUsers2.ParticipantOfRooms = new List<AppRooms> {appRooms1, appRooms2};
        appUsers1.HostOfRooms = new List<AppRooms> {appRooms1};
        appUsers2.HostOfRooms = new List<AppRooms> {appRooms2};



        appMessages1.AppRooms = appRooms1;
        appMessages1.AppUsers = appUsers1;
        appMessages2.AppRooms = appRooms1;
        appMessages2.AppUsers = appUsers2;

        appMessages3.AppRooms = appRooms2;
        appMessages3.AppUsers = appUsers2;
        appMessages4.AppRooms = appRooms2;
        appMessages4.AppUsers = appUsers1;

        context.Add(appMessages1);
        context.Add(appMessages2);
        context.Add(appMessages3);
        context.Add(appMessages4);

        context.Add(appUsers1);
        context.Add(appUsers2);

        context.Add(appRooms1);
        context.Add(appRooms2);

        context.Add(appTopics1);


        await userManager.CreateAsync(appUsers1, "Pa$$w0rd");
        await userManager.CreateAsync(appUsers2, "Pa$$w0rd2");
        await context.SaveChangesAsync();

    }


    public static async Task FillRoles(UserManager<AppUsers> userManager, RoleManager<AppRole> roleManager, DataContext context)
    {
        if (await roleManager.Roles.AnyAsync()) return;


        var roles = new List<AppRole>
        {
            new AppRole() {Name = "Member"},
            new AppRole() {Name = "Admin"},
            new AppRole() {Name = "Moderator"}
        };
        for (var i = 0; i < roles.Count; i++)
        {
            await roleManager.CreateAsync(roles[i]);
        }


        byte[] imageArray = File.ReadAllBytes("./wwwroot/assets/avatar.svg");
        string base64String = Convert.ToBase64String(imageArray);

        var defautAvatar = new AppImageObject
        {

            Url = "7848817827101636",
            ImageData = "data:image/svg+xml;base64," + base64String
        };

        context.ImageObjects.Add(defautAvatar);
        await context.SaveChangesAsync();


        // await userManager.Users.ForEachAsync(async user =>
        // {
        //     if (user.UserName != "admin")
        //     {
        //         await userManager.AddToRoleAsync(user, "Member");
        //     }
        //     else
        //     {
        //         await userManager.AddToRolesAsync(user, new []{"Admin","Moderator"});
        //     }
        // });

    }

}


