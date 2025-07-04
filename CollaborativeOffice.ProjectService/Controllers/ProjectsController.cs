using System.Security.Claims;
using CollaborativeOffice.Domain;
using CollaborativeOffice.ProjectService.DTOs;
using CollaborativeOffice.ProjectService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // --- Project Endpoints ---

    [HttpGet]
    public async Task<IActionResult> GetMyProjects()
    {
        var ownerId = GetCurrentUserId();
        if (ownerId == Guid.Empty) return Unauthorized();
        var projects = await _projectService.GetProjectsForUserAsync(ownerId);
        
        var projectsDto = projects.Select(p => new ProjectDto
        {
            Id = p.Id, Name = p.Name, Description = p.Description, CreatedAt = p.CreatedAt,
            Tasks = p.Tasks.Select(t => new TaskDto
            {
                Id = t.Id, Title = t.Title, Description = t.Description, IsCompleted = t.IsCompleted
            }).ToList()
        }).ToList();
        
        return Ok(projectsDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto request)
    {
        var ownerId = GetCurrentUserId();
        if (ownerId == Guid.Empty) return Unauthorized();
        var newProject = new Project
        {
            Id = Guid.NewGuid(), Name = request.Name, Description = request.Description,
            OwnerId = ownerId, CreatedAt = DateTime.UtcNow
        };
        var createdProject = await _projectService.CreateProjectAsync(newProject);
        var projectDto = new ProjectDto
        {
            Id = createdProject.Id, Name = createdProject.Name, Description = createdProject.Description,
            CreatedAt = createdProject.CreatedAt, Tasks = new List<TaskDto>()
        };
        return Ok(projectDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto request)
    {
        var ownerId = GetCurrentUserId();
        var success = await _projectService.UpdateProjectAsync(id, ownerId, request.Name, request.Description);
        if (!success) return NotFound("Project not found or you do not have permission.");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var ownerId = GetCurrentUserId();
        var result = await _projectService.DeleteProjectAsync(id, ownerId);
        if (!result.success)
        {
            return result.error != null && result.error.Contains("uncompleted tasks") 
                ? BadRequest(result.error) 
                : NotFound(result.error);
        }
        return NoContent();
    }

    // --- Task Endpoints ---

    [HttpPost("{projectId}/tasks")]
    public async Task<IActionResult> CreateTaskForProject(Guid projectId, [FromBody] CreateTaskDto request)
    {
        var ownerId = GetCurrentUserId();
        var result = await _projectService.CreateTaskAsync(projectId, ownerId, request.Title, request.Description);
        if (result.task == null) return NotFound(result.error);
        
        var taskDto = new TaskDto
        {
            Id = result.task.Id, Title = result.task.Title,
            Description = result.task.Description, IsCompleted = result.task.IsCompleted
        };
        return Ok(taskDto);
    }

    [HttpPatch("{projectId}/tasks/{taskId}/toggle")]
    public async Task<IActionResult> ToggleTaskCompletion(Guid projectId, Guid taskId)
    {
        var ownerId = GetCurrentUserId();
        var updatedTask = await _projectService.ToggleTaskCompletionAsync(projectId, taskId, ownerId);
        if (updatedTask == null) return NotFound("Task not found or you do not have permission.");
        
        var taskDto = new TaskDto
        {
            Id = updatedTask.Id, Title = updatedTask.Title,
            Description = updatedTask.Description, IsCompleted = updatedTask.IsCompleted
        };
        return Ok(taskDto);
    }

    [HttpDelete("{projectId}/tasks/{taskId}")]
    public async Task<IActionResult> DeleteTask(Guid projectId, Guid taskId)
    {
        var ownerId = GetCurrentUserId();
        var success = await _projectService.DeleteTaskAsync(projectId, taskId, ownerId);
        if (!success) return NotFound("Task not found or you do not have permission.");
        return NoContent();
    }
    
    [HttpGet("{projectId}/tasks/{taskId}")]
    public async Task<IActionResult> GetTaskById(Guid projectId, Guid taskId)
    {
        var ownerId = GetCurrentUserId();
        var result = await _projectService.GetTaskDetailsAsync(projectId, taskId, ownerId);
        if (result.task == null || !result.isOwner) return NotFound("Task not found or you do not have permission.");
        
        var taskDto = new TaskDto
        {
            Id = result.task.Id, Title = result.task.Title,
            Description = result.task.Description, IsCompleted = result.task.IsCompleted
        };
        return Ok(taskDto);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }
}