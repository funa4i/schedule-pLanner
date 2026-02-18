using SchedulePlannerBack.Domain.Entity;

namespace SchedulePlannerBack.Interfaces;

public interface IParticipantRepository
{
    void Save(Participant participant);
}