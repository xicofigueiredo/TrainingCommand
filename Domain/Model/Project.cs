using System.Security.AccessControl;
using Domain.Factory;

namespace Domain.Model;

public class Project : IProject
{
    public long Id { get; set; }
    private string _strName;
    public string Name
    {
        get { return _strName; }
    }

    private DateOnly _dateStart;
    public DateOnly StartDate
    {
        get { return _dateStart; }
    }
    
    private DateOnly? _dateEnd;
    public DateOnly? EndDate
    {
        get { return _dateEnd; }
    }

    protected Project() {}

    public Project(string strName, DateOnly dateStart, DateOnly? dateEnd)
    {
        if( !isValidParameters(strName, dateStart, dateEnd) ) {
            throw new ArgumentException("Invalid arguments.");
        }

        this._strName = strName;
        this._dateStart = dateStart;
        this._dateEnd = dateEnd;
    }

    public bool isValidParameters(string strName, DateOnly dateStart, DateOnly? dateEnd)
    { 
        if( strName==null || strName.Length > 50 || string.IsNullOrWhiteSpace(strName) ||
            (dateStart > dateEnd) )
        {
            return false;
        }
        return true;
    }

    public void SetName(string strName)
    {
        _strName = strName;
    }

    public void SetEndDate(DateOnly? endDate)
    {
        _dateEnd = endDate;
    }
}