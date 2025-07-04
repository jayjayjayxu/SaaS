using CollaborativeOffice.Domain;

namespace CollaborativeOffice.ProjectService.Repositories;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetProjectsByOwnerAsync(Guid ownerId);
    Task<Project?> GetProjectByIdAsync(Guid projectId);
    Task AddProjectAsync(Project project);
    void UpdateProject(Project project); // 更新和删除通常不是异步的，因为它们只改变实体状态
    void DeleteProject(Project project);
    Task<ProjectTask?> GetTaskByIdAsync(Guid taskId);
    Task AddTaskAsync(ProjectTask task);
    void DeleteTask(ProjectTask task);
    Task<bool> SaveChangesAsync(); // 用于一次性提交所有更改
}