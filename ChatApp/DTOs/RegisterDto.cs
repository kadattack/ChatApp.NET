using System.ComponentModel.DataAnnotations;

namespace ChatApp.DTOs;

public class RegisterDto
{
    [Required]
    [StringLength(32, MinimumLength = 4)]
    public string UserName { get; set; }

    [Required]
    [StringLength(32, MinimumLength = 4)]
    public string Email { get; set; }

    [Required]
    [StringLength(32, MinimumLength = 4)]
    public string Password { get; set; }
}