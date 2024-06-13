using BlazorSqlite.Data;
using BlazorSqlite.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorSqlite.Services;

[IgnoreAntiforgeryToken]
public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectService(ApplicationDbContext dbContext,
        AuthenticationStateProvider authenticationStateProvider,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _authenticationStateProvider = authenticationStateProvider;
        _userManager = userManager;
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userId = _userManager.GetUserId(user);

        return await _dbContext.Projects.Where(o => o.ClientId == new Guid(userId)).ToListAsync();
    }


    public async Task<Project?> GetProjectByIdAsync(int id) =>
        await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddProjectAsync(Project project)
    {
        try
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var userId = _userManager.GetUserId(user);

            project.ClientId = new Guid(userId);
            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {

            throw;
        }
    }

    public async Task UpdateProjectAsync(Project project)
    {
        var existingProject = _dbContext.Projects.FirstOrDefault(p => p.Id == project.Id);
        if (existingProject != null)
        {
            existingProject.Name = project.Name;
            existingProject.Description = project.Description;
            existingProject.DatabasePath = project.DatabasePath;

            _dbContext.Projects.Update(existingProject);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteProjectAsync(int id)
    {
        var project = _dbContext.Projects.FirstOrDefault(p => p.Id == id);
        if (project != null)
        {
            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync();
        }
    }
}
