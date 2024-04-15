using System.Net.NetworkInformation;

namespace DataModel.Repository;

using Microsoft.EntityFrameworkCore;

using DataModel.Model;
using DataModel.Mapper;

using Domain.Model;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore.ChangeTracking;
public class ProjectRepository : GenericRepository<Project>, IProjectRepository
{
    ProjectMapper _projectMapper;
    

    public ProjectRepository(AbsanteeContext context, ProjectMapper mapper) : base (context!)
    {
        _projectMapper = mapper;
    }
    
    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        try {
            IEnumerable<ProjectDataModel> projectsDataModel = await _context.Set<ProjectDataModel>()
                    .ToListAsync();

            IEnumerable<Project> projects = _projectMapper.ToDomain(projectsDataModel);

            return projects;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Project> GetProjectByIdAsync(long id)
    {
        try {
            ProjectDataModel projectDataModel = await _context.Set<ProjectDataModel>()
                    .FirstAsync(p => p.Id == id);

            Project project = _projectMapper.ToDomain(projectDataModel);

            return project;
        }
        catch
        {
            // throw;
            return null;
        }
    }

    public async Task<Project> Add(Project project)
    {
        try {
            ProjectDataModel projectDataModel = _projectMapper.ToDataModel(project);

            EntityEntry<ProjectDataModel> projectDataModelEntityEntry = _context.Set<ProjectDataModel>().Add(projectDataModel);
            
            await _context.SaveChangesAsync();

            ProjectDataModel projectDataModelSaved = projectDataModelEntityEntry.Entity;

            Project projectSaved = _projectMapper.ToDomain(projectDataModelSaved);
            
            return projectSaved;    
        }
        catch
        {
            throw;
        }
    }

    public async Task<Project> Update(Project project, List<string> errorMessages)
    {
        try {
            ProjectDataModel projectDataModel = await _context.Set<ProjectDataModel>()
                    .FirstAsync(c => c.Id == project.Id);

            _projectMapper.UpdateDataModel(projectDataModel, project);

            _context.Entry(projectDataModel).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return project;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ProjectExists(project.Id, project.Name))
            {
                errorMessages.Add("Not found");
                
                return null;
            }
            else
            {
                throw;
            }

            return null;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ProjectExists(long id, string name)
    {
        return await _context.Set<ProjectDataModel>().AnyAsync(e => e.Id == id || e.Name == name);
    }
}