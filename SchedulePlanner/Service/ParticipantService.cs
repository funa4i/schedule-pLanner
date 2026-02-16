using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Repository;
using SchedulePlannerBack.Util;

namespace SchedulePlannerBack.Service;

public class ParticipantService
{
    private readonly ParticipantRepository _participantRepository;
    private readonly ILogger _logger;

    public ParticipantService(ParticipantRepository participantRepository, ILogger logger)
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