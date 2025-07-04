using CollaborativeOffice.Domain;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeOffice.IdentityService.Data;

public class ApplicationDbContext : DbContext
{
    // 这个构造函数是让依赖注入系统能够配置和传入数据库连接信息的关键
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // 这行代码告诉EF Core，我们希望有一个名为“Users”的表，
    // 它的结构对应我们之前创建的User类。
    public DbSet<User> Users { get; set; }
}