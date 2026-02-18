using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Interfaces;
using SchedulePlannerBack.Interfaces.Application;
using SchedulePlannerBack.Repository;

namespace SchedulePlannerBack.Application;

public class ParticipantService : IParticipantService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly ILogger _logger;

    public ParticipantService(IParticipantRepository participantRepository, ILogger logger)
    {;
        _participantRepository = participantRepository;
        _logger = logger;
    }

    public void SaveParticipant(Participant participant)
    {
        _logger.LogInformation("Saving participant: guestName={ParticipantGuestName} " +
                               "eventId={ParticipantEventId} userId={UserId} ", 
            participant.GuestName, participant.EventId, participant.UserId);
        _participantRepository.Save(participant);
    }
}