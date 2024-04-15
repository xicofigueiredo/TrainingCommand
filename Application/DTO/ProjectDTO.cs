namespace Application.DTO;

using Domain.Model;

public class ProjectDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public ProjectDTO()
    {
    }

    public ProjectDTO(long id, string name, DateOnly startDate, DateOnly? endDate)
    {
        Id = id;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static ProjectDTO ToDTO(Project project)
    {
        ProjectDTO projectDto = new ProjectDTO(project.Id, project.Name, project.StartDate, project.EndDate);
        return projectDto;
    }
    
    public static IEnumerable<ProjectDTO> ToDTO(IEnumerable<Project> projects)
    {
        List<ProjectDTO> projectsDTO = new List<ProjectDTO>();
        foreach (Project project in projects)
        {
            ProjectDTO projectDTO = ToDTO(project);
            projectsDTO.Add(projectDTO);
        }
        return projectsDTO;
    }

    public static Project ToDomain(ProjectDTO projectDto)
    {
        Project project = new Project(projectDto.Name, projectDto.StartDate, projectDto.EndDate);
        return project;
    }

    public static Project UpdateToDomain(Project project, ProjectDTO projectDTO)
    {
        project.SetName(projectDTO.Name);
        project.SetEndDate(projectDTO.EndDate);
        return project;
    }
}