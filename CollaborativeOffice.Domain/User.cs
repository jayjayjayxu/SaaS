namespace CollaborativeOffice.Domain;

public class User
{
    public Guid Id { get; set; } // 全局唯一ID
    public string Username { get; set; } = string.Empty; // 用户名
    public string PasswordHash { get; set; } = string.Empty; // 存储加密后的密码
    public DateTime CreatedAt { get; set; } // 创建时间

    // 构造函数，用于初始化
    public User()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}