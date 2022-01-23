using System.ComponentModel.DataAnnotations;
using ChatApp.Entities;

namespace ChatApp.DTOs;

public class ProfileUpdateDto
{
    [Required]
    public string UserName { get; set; }
    public string Bio { get; set; }
    [Required]
    public string Email { get; set; }

    public AppImageObject? AvatarImageObject { get; set; }
}