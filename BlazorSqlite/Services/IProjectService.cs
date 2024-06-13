using BlazorSqlite.Models;

namespace BlazorSqlite.Services
{
    public interface IProjectService
    {
        Task AddProjectAsync(Project project);
        Task DeleteProjectAsync(int id);
        Task<Project?> GetProjectByIdAsync(int id);
        Task<List<Project>> GetProjectsAsync();
        Task UpdateProjectAsync(Project project);
    }
}