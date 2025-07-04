namespace CollaborativeOffice.Domain;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; } // 项目创建者的用户ID
    public DateTime CreatedAt { get; set; }
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}