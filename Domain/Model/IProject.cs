using Domain.Factory;

namespace Domain.Model;

public interface IProject
{
    long Id {get;}
    string Name {get;}
    DateOnly StartDate {get;}
    DateOnly? EndDate {get;}
    // public Associate addAssociate(IAssociateFactory aFactory, IColaborator colaborator, DateOnly startDate, DateOnly? endDate);
    public bool isValidParameters(string strName, DateOnly dateStart, DateOnly? dateEnd); 
    // public List<IAssociate> getListByColaborator(IColaborator colaborator);
    // public List<IAssociate> getListByColaboratorInRange(IColaborator colaborator, DateOnly startDate, DateOnly? endDate);
    // public List<IColaborator> getListColaboratorByProject();
}