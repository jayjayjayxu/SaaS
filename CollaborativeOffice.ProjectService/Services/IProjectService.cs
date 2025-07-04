using CollaborativeOffice.Domain;
namespace CollaborativeOffice.ProjectService.Services;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetProjectsForUserAsync(Guid ownerId);
    Task<(Project? project, bool isOwner)> GetProjectDetailsAsync(Guid projectId, Guid ownerId);
    Task<Project> CreateProjectAsync(Project project);
    Task<bool> UpdateProjectAsync(Guid projectId, Guid ownerId, string name, string? description);
    Task<(ProjectTask? task, string? error)> CreateTaskAsync(Guid projectId, Guid ownerId, string title, string? description);
    Task<ProjectTask?> ToggleTaskCompletionAsync(Guid projectId, Guid taskId, Guid ownerId);
    Task<bool> DeleteTaskAsync(Guid projectId, Guid taskId, Guid ownerId);
    Task<(ProjectTask? task, bool isOwner)> GetTaskDetailsAsync(Guid projectId, Guid taskId, Guid ownerId);
    Task<(bool success, string? error)> DeleteProjectAsync(Guid projectId, Guid ownerId);
}