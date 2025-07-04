using System.ComponentModel.DataAnnotations;

namespace CollaborativeOffice.IdentityService.DTOs;

public class RegisterRequestDto
{
    [Required] // 表示此字段是必填的
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}