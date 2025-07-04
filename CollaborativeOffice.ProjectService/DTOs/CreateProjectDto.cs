using System.ComponentModel.DataAnnotations;

namespace CollaborativeOffice.ProjectService.DTOs;

public class CreateProjectDto
{
    [Required(ErrorMessage = "项目名称不能为空")]
    [StringLength(100, ErrorMessage = "项目名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "项目描述长度不能超过500个字符")]
    public string? Description { get; set; }
}