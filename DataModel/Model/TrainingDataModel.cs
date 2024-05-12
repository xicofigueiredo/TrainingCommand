using System.ComponentModel.DataAnnotations;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DataModel.Model;

[Index(nameof(Name), IsUnique = true)]
public class TrainingDataModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public TrainingDataModel()
    { }

    public TrainingDataModel(ITraining training)
    {
        Id = training.Id;
        Name = training.Name;
        StartDate = training.StartDate;
        EndDate = training.EndDate;
    }
}