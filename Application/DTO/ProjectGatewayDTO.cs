using System;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json;

namespace Application.DTO
{
    public class ProjectGatewayDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public ProjectGatewayDTO()
        {
        }

        public ProjectGatewayDTO( long id, string name, DateOnly startDate, DateOnly? endDate)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public static string Serialize(ProjectDTO projectDTO)
        {
            ProjectGatewayDTO projectGateway = new ProjectGatewayDTO(projectDTO.Id, projectDTO.Name, projectDTO.StartDate, projectDTO.EndDate);
            var jsonMessage = JsonConvert.SerializeObject(projectGateway);
            return jsonMessage;
        }

        public static ProjectGatewayDTO Deserialize(string projectDTOString)
        {
            return JsonConvert.DeserializeObject<ProjectGatewayDTO>(projectDTOString);
        }

        public static ProjectDTO ToDTO(string projectDTOString)
        {
            ProjectGatewayDTO projectGatewayDTO = Deserialize(projectDTOString);
            ProjectDTO projectDTO = new ProjectDTO(projectGatewayDTO.Id, projectGatewayDTO.Name, projectGatewayDTO.StartDate,
                projectGatewayDTO.EndDate);
            return projectDTO;
        }
    }
}