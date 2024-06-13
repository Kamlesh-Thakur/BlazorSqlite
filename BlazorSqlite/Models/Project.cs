using System.ComponentModel.DataAnnotations;

namespace BlazorSqlite.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    public Guid ClientId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string DatabasePath { get; set; } = string.Empty;
}
