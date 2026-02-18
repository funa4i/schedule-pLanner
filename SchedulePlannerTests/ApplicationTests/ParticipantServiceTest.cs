using EntityFramework.Exceptions.Common;
using Microsoft.Extensions.Logging;
using Moq;
using SchedulePlannerBack.Application;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Repository;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Interfaces;
using SchedulePlannerBack.Repository;
using SchedulePlannerBack.Service;

namespace SchedulePlannerTests.ApplicationTests;

[TestFixture]
public class ParticipantServiceTest
{
    private Mock<ILogger> _loggerMock;
    private Mock<IParticipantRepository> _participantRepositoryMock;
    private ParticipantService _participantService;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger>();
        _participantRepositoryMock = new Mock<IParticipantRepository>();
        _participantService = new ParticipantService(
            _participantRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public void SaveParticipant_CallsRepositorySave()
    {
        var participant = CreateParticipant(new List<EventDate> { CreateEventDate() });

        _participantService.SaveParticipant(participant);

        _participantRepositoryMock.Verify(
            x => x.Save(participant),
            Times.Once);
    }
    
    [Test]
    public void SaveParticipant_ThrowsStorageException()
    {
        var participant = CreateParticipant(new List<EventDate>());

        _participantRepositoryMock
            .Setup(x => x.Save(It.IsAny<Participant>()))
            .Throws(new StorageException(new Exception("db error")));

        Assert.Throws<StorageException>(() =>
            _participantService.SaveParticipant(participant));
    }
    
    [Test]
    public void SaveParticipant_ThrowsUniqueException()
    {
        var participant = CreateParticipant(new List<EventDate>());

        _participantRepositoryMock
            .Setup(x => x.Save(It.IsAny<Participant>()))
            .Throws(new UniqueConstraintException());

        Assert.Throws<UniqueConstraintException>(() =>
            _participantService.SaveParticipant(participant));
    }

    
    private Participant CreateParticipant(List<EventDate> eventDates)
    {
        return new Participant
        {
            Id = 1,
            GuestName = "Guest",
            EventId = 1,
            UserId = 1,
            EventDates = eventDates
        };
    }

    private EventDate CreateEventDate()
    {
        return new EventDate
        {
            Id = 1,
            ParticipantId = 1,
            DateFrom = DateTime.UtcNow.AddDays(-1),
            DateTo = DateTime.UtcNow.AddDays(1)
        };
    }
}