namespace Application.DTO;

using Domain.Model;
using System.Linq;

public class TrainingDTO
{
    public long Id { get; }
    public string Name { get; }
    public DateOnly StartDate { get; }
    public DateOnly? EndDate { get; }

    public TrainingDTO(long id, string name, DateOnly startDate, DateOnly? endDate)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        StartDate = startDate;
        EndDate = endDate;
    }

    public static TrainingDTO ToDTO(Training training)
    {
        if (training == null) throw new ArgumentNullException(nameof(training));
        return new TrainingDTO(training.Id, training.Name, training.StartDate, training.EndDate);
    }
    
    public static IEnumerable<TrainingDTO> ToDTO(IEnumerable<Training> trainings)
        => trainings?.Select(ToDTO) ?? throw new ArgumentNullException(nameof(trainings));

    public static Training ToDomain(TrainingDTO trainingDto)
    {
        if (trainingDto == null) throw new ArgumentNullException(nameof(trainingDto));
        return new Training(trainingDto.Name, trainingDto.StartDate, trainingDto.EndDate);
    }

    public static Training UpdateToDomain(Training training, TrainingDTO trainingDTO)
    {
        if (training == null) throw new ArgumentNullException(nameof(training));
        if (trainingDTO == null) throw new ArgumentNullException(nameof(trainingDTO));

        training.SetName(trainingDTO.Name);
        training.SetEndDate(trainingDTO.EndDate);
        return training;
    }
}
