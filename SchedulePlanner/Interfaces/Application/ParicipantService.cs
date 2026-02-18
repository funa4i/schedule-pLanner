using SchedulePlannerBack.Domain.Entity;

namespace SchedulePlannerBack.Interfaces.Application;

public interface IParticipantService
{
    void SaveParticipant(Participant participant);
}