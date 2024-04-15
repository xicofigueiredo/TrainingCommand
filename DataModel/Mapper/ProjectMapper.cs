namespace DataModel.Mapper;

using DataModel.Model;

using Domain.Model;
using Domain.Factory;

public class ProjectMapper
{
    private IProjectFactory _projectFactory;

    public ProjectMapper(IProjectFactory projectFactory)
    {
        _projectFactory = projectFactory;
    }
    
    public Project ToDomain(ProjectDataModel projectDM)
    {
        Project projectDomain = _projectFactory.NewProject(projectDM.Name, projectDM.StartDate, projectDM.EndDate);

        projectDomain.Id = projectDM.Id;

        return projectDomain;
    }

    public IEnumerable<Project> ToDomain(IEnumerable<ProjectDataModel> projectsDataModel)
    {

        List<Project> colabsDomain = new List<Project>();

        foreach(ProjectDataModel projectDataModel in projectsDataModel)
        {
            Project projectDomain = ToDomain(projectDataModel);

            colabsDomain.Add(projectDomain);
        }

        return colabsDomain.AsEnumerable();
    }

    public ProjectDataModel ToDataModel(Project colab)
    {
        ProjectDataModel projectDataModel = new ProjectDataModel(colab);

        return projectDataModel;
    }


    public bool UpdateDataModel(ProjectDataModel projectDataModel, Project projectDomain)
    {
        projectDataModel.Name = projectDomain.Name;

        // pode ser necessário mais atualizações, e com isso o retorno não ser sempre true
        // contudo, porque projectDataModel está a ser gerido pelo DbContext, para atualizarmos a DB, é este que tem de ser alterado, e não criar um novo
        projectDataModel.EndDate = projectDomain.EndDate;
        
        return true;
    }
}