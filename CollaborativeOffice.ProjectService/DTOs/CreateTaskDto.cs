using System.ComponentModel.DataAnnotations;

namespace CollaborativeOffice.ProjectService.DTOs;

public class CreateTaskDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
}