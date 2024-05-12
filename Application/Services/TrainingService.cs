using Application.DTO;
using Domain.IRepository;
using Domain.Model;
using Gateway;

namespace Application.Services;

public class TrainingService
{
    private readonly ITrainingRepository _trainingRepository;

    private readonly TrainingGateway _trainingGateway;
    private readonly TrainingGatewayUpdate _trainingGatewayUpdate;
    private readonly SagaGateway _sagaGateway;

    public TrainingService(ITrainingRepository trainingRepository, TrainingGateway trainingGateway, TrainingGatewayUpdate trainingGatewayUpdate)
    {
        _trainingRepository = trainingRepository;
        _trainingGateway = trainingGateway;
        _trainingGatewayUpdate = trainingGatewayUpdate;
    }

    public async Task<IEnumerable<TrainingDTO>> GetAll()
    {
        IEnumerable<Training> trainings = await _trainingRepository.GetTrainingsAsync();

        if (trainings is not null)
        {
            IEnumerable<TrainingDTO> trainingsDTO = TrainingDTO.ToDTO(trainings);
            return trainingsDTO;
        }

        return null;
    }

    public async Task<TrainingDTO> GetById(long id)
    {
        Training training = await _trainingRepository.GetTrainingByIdAsync(id);

        if (training is not null)
        {
            TrainingDTO trainingDTO = TrainingDTO.ToDTO(training);
            return trainingDTO;
        }

        return null;
    }

    public async Task<TrainingDTO> AddFromAMQP(TrainingDTO trainingDTO, List<string> errorMessages)
    {
        bool pExists = await _trainingRepository.TrainingExists(trainingDTO.Id, trainingDTO.Name);
        if (pExists)
        {
            errorMessages.Add("Training already exists.");
            return null;
        }

        try
        {
            Training training = TrainingDTO.ToDomain(trainingDTO);
            Training trainingSaved = await _trainingRepository.Add(training);
            TrainingDTO trainingDTOSaved = TrainingDTO.ToDTO(trainingSaved);
            
            return trainingDTOSaved;
        }
        catch (ArgumentException ex)
        {
            errorMessages.Add(ex.Message);
            return null;
        }
    }

    public async Task<TrainingDTO> AddFromRest(TrainingDTO trainingDTO, List<string> errorMessages)
        {
            TrainingDTO trainingDTOSaved = await AddFromAMQP(trainingDTO, errorMessages);
    
            if (trainingDTOSaved is not null)
            {
                string jsonMessage = TrainingGatewayDTO.Serialize(trainingDTOSaved);
                _trainingGateway.publish(jsonMessage);
            }
            return trainingDTOSaved;
        }
    
    public async Task<bool> Update(long id, TrainingDTO trainingDTO, List<string> errorMessages, bool cameFromSwagger)
    {
        Training training = await _trainingRepository.GetTrainingByIdAsync(id);

        if (training is not null)
        {
            TrainingDTO.UpdateToDomain(training, trainingDTO);
            await _trainingRepository.Update(training, errorMessages);

            if (cameFromSwagger)
            {
                string jsonMessage = TrainingGatewayDTO.Serialize(trainingDTO);
                _trainingGatewayUpdate.publish(jsonMessage);
            }
            
            return true;
        }
        else
        {
            errorMessages.Add("Not found");
            return false;
        }
    }
    
    public void NothingToSeeHereJustReturnOKForTesting()
    {
        string jsonMessage = "Nothing to see here, just return OK for testing.";
        _sagaGateway.publish(jsonMessage);
    }
}