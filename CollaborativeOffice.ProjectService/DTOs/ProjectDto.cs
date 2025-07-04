namespace CollaborativeOffice.ProjectService.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    // 关键：这里的Tasks是TaskDto类型的列表，而不是ProjectTask
    public List<TaskDto> Tasks { get; set; } = new();
}