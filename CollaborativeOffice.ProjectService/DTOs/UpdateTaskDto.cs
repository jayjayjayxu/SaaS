using System.ComponentModel.DataAnnotations;

namespace CollaborativeOffice.ProjectService.DTOs;

public class UpdateTaskDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
    
    public bool IsCompleted { get; set; }
}