using Domain.Model;

namespace Domain.Factory;

public class TrainingFactory : ITrainingFactory
{
    public Training NewTraining(string strName, DateOnly dateStart, DateOnly? dateEnd)
    {
        return new Training(strName, dateStart, dateEnd);
    }
}