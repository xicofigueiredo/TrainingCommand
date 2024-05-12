using System;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json;

namespace Application.DTO
{
    public class TrainingGatewayDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public TrainingGatewayDTO()
        { }

        public TrainingGatewayDTO( long id, string name, DateOnly startDate, DateOnly? endDate)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public static string Serialize(TrainingDTO trainingDTO)
        {
            TrainingGatewayDTO trainingGateway = new TrainingGatewayDTO(trainingDTO.Id, trainingDTO.Name, trainingDTO.StartDate, trainingDTO.EndDate);
            var jsonMessage = JsonConvert.SerializeObject(trainingGateway);
            return jsonMessage;
        }

        public static TrainingGatewayDTO Deserialize(string trainingDTOString)
        {
            return JsonConvert.DeserializeObject<TrainingGatewayDTO>(trainingDTOString);
        }

        public static TrainingDTO ToDTO(string trainingDTOString)
        {
            TrainingGatewayDTO trainingGatewayDTO = Deserialize(trainingDTOString);
            TrainingDTO trainingDTO = new TrainingDTO(trainingGatewayDTO.Id, trainingGatewayDTO.Name, trainingGatewayDTO.StartDate,
                trainingGatewayDTO.EndDate);
            return trainingDTO;
        }
    }
}