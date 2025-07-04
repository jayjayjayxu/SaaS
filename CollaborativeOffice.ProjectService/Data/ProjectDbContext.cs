using CollaborativeOffice.Domain;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeOffice.ProjectService.Data;

public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
    {
    }

    // 注册新的领域模型
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
}