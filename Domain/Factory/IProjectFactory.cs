using Domain.Model;

namespace Domain.Factory;

public interface IProjectFactory
{
    Project NewProject(string strName, DateOnly dateStart, DateOnly? dateEnd);
}