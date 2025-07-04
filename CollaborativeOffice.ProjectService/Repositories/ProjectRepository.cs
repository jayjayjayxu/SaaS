using CollaborativeOffice.ProjectService.Data;
using CollaborativeOffice.Domain;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeOffice.ProjectService.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ProjectDbContext _context;

    public ProjectRepository(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetProjectsByOwnerAsync(Guid ownerId)
    {
        return await _context.Projects
            .Where(p => p.OwnerId == ownerId)
            .Include(p => p.Tasks)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(Guid projectId)
    {
        return await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task AddProjectAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
    }

    public void UpdateProject(Project project)
    {
        _context.Projects.Update(project);
    }

    public void DeleteProject(Project project)
    {
        _context.Projects.Remove(project);
    }
    
    public async Task<ProjectTask?> GetTaskByIdAsync(Guid taskId)
    {
        // 获取任务时，也需要把其所属的项目加载进来，以便进行权限验证
        return await _context.ProjectTasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task AddTaskAsync(ProjectTask task)
    {
        await _context.ProjectTasks.AddAsync(task);
    }

    public void DeleteTask(ProjectTask task)
    {
        _context.ProjectTasks.Remove(task);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}