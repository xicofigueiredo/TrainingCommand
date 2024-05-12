namespace DataModel.Mapper;

using DataModel.Model;

using Domain.Model;
using Domain.Factory;

public class TrainingMapper
{
    private ITrainingFactory _trainingFactory;

    public TrainingMapper(ITrainingFactory trainingFactory)
    {
        _trainingFactory = trainingFactory;
    }
    
    public Training ToDomain(TrainingDataModel trainingDM)
    {
        Training trainingDomain = _trainingFactory.NewTraining(trainingDM.Name, trainingDM.StartDate, trainingDM.EndDate);

        trainingDomain.Id = trainingDM.Id;

        return trainingDomain;
    }

    public IEnumerable<Training> ToDomain(IEnumerable<TrainingDataModel> trainingsDataModel)
    {

        List<Training> colabsDomain = new List<Training>();

        foreach(TrainingDataModel trainingDataModel in trainingsDataModel)
        {
            Training trainingDomain = ToDomain(trainingDataModel);

            colabsDomain.Add(trainingDomain);
        }

        return colabsDomain.AsEnumerable();
    }

    public TrainingDataModel ToDataModel(Training colab)
    {
        TrainingDataModel trainingDataModel = new TrainingDataModel(colab);

        return trainingDataModel;
    }


    public bool UpdateDataModel(TrainingDataModel trainingDataModel, Training trainingDomain)
    {
        trainingDataModel.Name = trainingDomain.Name;

        // pode ser necessário mais atualizações, e com isso o retorno não ser sempre true
        // contudo, porque trainingDataModel está a ser gerido pelo DbContext, para atualizarmos a DB, é este que tem de ser alterado, e não criar um novo
        trainingDataModel.EndDate = trainingDomain.EndDate;
        
        return true;
    }
}