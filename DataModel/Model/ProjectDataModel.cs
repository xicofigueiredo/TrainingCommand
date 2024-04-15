using System.ComponentModel.DataAnnotations;
using Domain.Model;

namespace DataModel.Model;
public class ProjectDataModel
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public ProjectDataModel()
    { }

    public ProjectDataModel(IProject project)
    {
        Id = project.Id;
        Name = project.Name;
        StartDate = project.StartDate;
        EndDate = project.EndDate;
    }
}