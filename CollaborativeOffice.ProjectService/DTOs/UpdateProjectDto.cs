using System.ComponentModel.DataAnnotations;

namespace CollaborativeOffice.ProjectService.DTOs;

public class UpdateProjectDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}