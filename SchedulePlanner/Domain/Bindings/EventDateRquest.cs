using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Util;

namespace SchedulePlannerBack.Domain.Bindings;

public class EventDateRequest : IValidation
{
    public EventDateRequest(DateTime dateFrom, DateTime dateTo)
    {
        DateFrom = dateFrom;
        DateTo = dateTo;
    }

    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    
    public void Validate()
    {
        if (DateFrom > DateTo)
            throw new IncorrectDatesException(DateFrom, DateTo);
    }
}