using CollaborativeOffice.Domain;
using CollaborativeOffice.ProjectService.Repositories;

namespace CollaborativeOffice.ProjectService.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<Project>> GetProjectsForUserAsync(Guid ownerId)
    {
        return await _projectRepository.GetProjectsByOwnerAsync(ownerId);
    }

    public async Task<Project> CreateProjectAsync(Project project)
    {
        await _projectRepository.AddProjectAsync(project);
        await _projectRepository.SaveChangesAsync();
        return project;
    }

    public async Task<(bool success, string? error)> DeleteProjectAsync(Guid projectId, Guid ownerId)
    {
        var project = await GetProjectDetailsAsync(projectId, ownerId);
        if (!project.isOwner || project.project == null)
        {
            return (false, "Project not found or you do not have permission.");
        }

        bool hasUncompletedTasks = project.project.Tasks.Any(t => !t.IsCompleted);
        if (hasUncompletedTasks)
        {
            return (false, "Cannot delete! This project has uncompleted tasks.");
        }

        _projectRepository.DeleteProject(project.project);
        await _projectRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(Project? project, bool isOwner)> GetProjectDetailsAsync(Guid projectId, Guid ownerId)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId);
        if (project == null) return (null, false);
        return (project, project.OwnerId == ownerId);
    }

    public async Task<bool> UpdateProjectAsync(Guid projectId, Guid ownerId, string name, string? description)
    {
        var projectDetails = await GetProjectDetailsAsync(projectId, ownerId);
        if (!projectDetails.isOwner || projectDetails.project == null)
        {
            return false;
        }

        projectDetails.project.Name = name;
        projectDetails.project.Description = description;

        // EF Core 知道这个实体是被追踪的，所以不需要调用Update方法
        // _projectRepository.UpdateProject(projectDetails.project); 
        await _projectRepository.SaveChangesAsync();
        return true;
    }
      public async Task<(ProjectTask? task, string? error)> CreateTaskAsync(Guid projectId, Guid ownerId, string title, string? description)
    {
        var projectDetails = await GetProjectDetailsAsync(projectId, ownerId);
        if (!projectDetails.isOwner)
        {
            return (null, "Project not found or you do not have permission.");
        }

        var newTask = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            IsCompleted = false,
            ProjectId = projectId
        };
        
        await _projectRepository.AddTaskAsync(newTask);
        await _projectRepository.SaveChangesAsync();

        return (newTask, null);
    }

    public async Task<ProjectTask?> ToggleTaskCompletionAsync(Guid projectId, Guid taskId, Guid ownerId)
    {
        var task = await _projectRepository.GetTaskByIdAsync(taskId);
        if (task == null || task.ProjectId != projectId || task.Project?.OwnerId != ownerId)
        {
            return null; // Not found or no permission
        }

        task.IsCompleted = !task.IsCompleted;
        await _projectRepository.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteTaskAsync(Guid projectId, Guid taskId, Guid ownerId)
    {
        var task = await _projectRepository.GetTaskByIdAsync(taskId);
        if (task == null || task.ProjectId != projectId || task.Project?.OwnerId != ownerId)
        {
            return false;
        }

        _projectRepository.DeleteTask(task);
        await _projectRepository.SaveChangesAsync();
        return true;
    }

    public async Task<(ProjectTask? task, bool isOwner)> GetTaskDetailsAsync(Guid projectId, Guid taskId, Guid ownerId)
    {
        var task = await _projectRepository.GetTaskByIdAsync(taskId);
        if (task == null || task.ProjectId != projectId)
        {
            return (null, false);
        }
        return (task, task.Project?.OwnerId == ownerId);
    }
}