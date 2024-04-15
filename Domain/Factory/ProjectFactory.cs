using Domain.Model;

namespace Domain.Factory;

public class ProjectFactory : IProjectFactory
{
    public Project NewProject(string strName, DateOnly dateStart, DateOnly? dateEnd)
    {
        return new Project(strName, dateStart, dateEnd);
    }
}