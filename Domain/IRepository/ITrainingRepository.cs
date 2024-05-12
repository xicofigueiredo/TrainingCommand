using Domain.Model;

namespace Domain.IRepository;

public interface ITrainingRepository  : IGenericRepository<Training>
{
    Task<IEnumerable<Training>> GetTrainingsAsync();
    Task<Training> GetTrainingByIdAsync(long id);
    Task<Training> Add(Training training);
    Task<Training> Update(Training training, List<string> errorMessages);
    Task<bool> TrainingExists(long id, string name);
}