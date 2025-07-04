namespace CollaborativeOffice.Domain;

public class ProjectTask
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public Guid ProjectId { get; set; } // 所属项目的ID
    public Project? Project { get; set; }
}