using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ChatApp.Entities;

public class AppUsers : IdentityUser<int>

{
    // public int Id { get; set; }
    // [Required]
    // public string UserName { get; set; }
    [Required]
    public string Email { get; set; }
    public string Bio { get; set; } = "";

    public ICollection<AppRooms> HostOfRooms { get; set; } = new List<AppRooms>();
    public ICollection<AppMessages> AppMessages { get; set; } = new List<AppMessages>();

    public ICollection<AppRooms> ParticipantOfRooms { get; set; } = new List<AppRooms>();
    public string AvatarUrl { get; set; } = "./assets/avatar.svg";

    public DateTime Created { get; set; } = DateTime.Now;

    public DateTime Updated { get; set; } = DateTime.Now;

    // public ICollection<AppRole> AppRoles { get; set; }

    // [Required]
    // public byte[] PasswordHash { get; set; }
    // [Required]
    // public byte[] PasswordSalt { get; set; }



}