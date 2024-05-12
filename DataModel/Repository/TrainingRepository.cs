using System.Net.NetworkInformation;

namespace DataModel.Repository;

using Microsoft.EntityFrameworkCore;

using DataModel.Model;
using DataModel.Mapper;

using Domain.Model;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore.ChangeTracking;
public class TrainingRepository : GenericRepository<Training>, ITrainingRepository
{
    TrainingMapper _trainingMapper; 
    

    public TrainingRepository(AbsanteeContext context, TrainingMapper mapper) : base (context!)
    {
        _trainingMapper = mapper;
    }
    
    public async Task<IEnumerable<Training>> GetTrainingsAsync()
    {
        try {
            IEnumerable<TrainingDataModel> trainingsDataModel = await _context.Set<TrainingDataModel>()
                    .ToListAsync();

            IEnumerable<Training> trainings = _trainingMapper.ToDomain(trainingsDataModel);

            return trainings;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Training> GetTrainingByIdAsync(long id)
    {
        try {
            TrainingDataModel trainingDataModel = await _context.Set<TrainingDataModel>()
                    .FirstAsync(p => p.Id == id);

            Training training = _trainingMapper.ToDomain(trainingDataModel);

            return training;
        }
        catch
        {
            // throw;
            return null;
        }
    }

    public async Task<Training> Add(Training training)
    {
        try {
            TrainingDataModel trainingDataModel = _trainingMapper.ToDataModel(training);
    
            EntityEntry<TrainingDataModel> trainingDataModelEntityEntry = _context.Set<TrainingDataModel>().Add(trainingDataModel);
            
            await _context.SaveChangesAsync();
    
            TrainingDataModel trainingDataModelSaved = trainingDataModelEntityEntry.Entity;
    
            Training trainingSaved = _trainingMapper.ToDomain(trainingDataModelSaved);
            
            return trainingSaved;    
        }
        catch
        {
            throw;
        }
    }
    
    public async Task<Training> Update(Training training, List<string> errorMessages)
    {
        try {
            TrainingDataModel trainingDataModel = await _context.Set<TrainingDataModel>()
                    .FirstAsync(c => c.Id == training.Id);
    
            _trainingMapper.UpdateDataModel(trainingDataModel, training);
    
            _context.Entry(trainingDataModel).State = EntityState.Modified;
    
            await _context.SaveChangesAsync();
    
            return training;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await TrainingExists(training.Id, training.Name))
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
    
    public async Task<bool> TrainingExists(long id, string name)
    {
        return await _context.Set<TrainingDataModel>().AnyAsync(e => e.Id == id || e.Name == name);
    }
}