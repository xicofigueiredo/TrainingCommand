using Application.DTO;
using Domain.IRepository;
using Domain.Model;
using Gateway;

namespace Application.Services;

public class ProjectAMQPService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ProjectGateway _projectGateway;
    public ProjectAMQPService(IProjectRepository projectRepository, ProjectGateway projectGateway)
    {
        _projectRepository = projectRepository;
        _projectGateway = projectGateway;
    }
    
    public async Task<ProjectDTO> Add(ProjectDTO projectDTO, List<string> errorMessages)
    {
        bool pExists = await _projectRepository.ProjectExists(projectDTO.Id, projectDTO.Name);
        if (pExists)
        {
            errorMessages.Add("Project already exists.");
            return null;
        }

        try
        {
            Project project = ProjectDTO.ToDomain(projectDTO);
            Project projectSaved = await _projectRepository.Add(project);
            ProjectDTO projectDTOSaved = ProjectDTO.ToDTO(projectSaved);

            string jsonMessage = ProjectGatewayDTO.Serialize(projectDTOSaved);
            _projectGateway.publish(jsonMessage);
            
            return projectDTOSaved;
        }
        catch (ArgumentException ex)
        {
            errorMessages.Add(ex.Message);
            return null;
        }
    }
}