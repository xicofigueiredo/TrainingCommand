using Domain.Model;

namespace Domain.IRepository;

public interface IProjectRepository  : IGenericRepository<Project>
{
    Task<IEnumerable<Project>> GetProjectsAsync();
    Task<Project> GetProjectByIdAsync(long id);
    Task<Project> Add(Project project);
    Task<Project> Update(Project project, List<string> errorMessages);
    Task<bool> ProjectExists(long id, string name);
}