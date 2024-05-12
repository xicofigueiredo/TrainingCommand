using Domain.Model;

namespace Domain.Factory;

public interface ITrainingFactory
{
    Training NewTraining(string strName, DateOnly dateStart, DateOnly? dateEnd);
}