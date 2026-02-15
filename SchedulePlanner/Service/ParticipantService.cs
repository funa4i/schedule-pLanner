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

    private void SaveParticipant(Participant participant, long? userId)
    {
        _logger.LogInformation("Saving participant: guestName={ParticipantGuestName} " +
                               "eventId={ParticipantEventId} userId={UserId} ", 
            participant.GuestName, participant.EventId, userId);
        participant.UserId = userId;
        _participantRepository.Save(participant);
    }
}