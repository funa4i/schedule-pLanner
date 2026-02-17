using AutoMapper;
using EntityFramework.Exceptions.Common;
using SchedulePlannerBack.Controller;
using SchedulePlannerBack.Domain.Bindings;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Views;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Service;
using SchedulePlannerBack.Util;

namespace SchedulePlannerBack.Adapters;

public class ParticipantAdapter
{
    private readonly ParticipantService _participantService;
    private readonly ILogger<ParticipantAdapter> _logger;
    private readonly UserAuthenticationProvider  _userAuthentication;
    private readonly IMapper _mapper;

    public ParticipantAdapter(ParticipantService participantService, ILogger<ParticipantAdapter> logger,
        UserAuthenticationProvider userAuthentication)
    {
        _participantService = participantService;
        _logger = logger;
        _userAuthentication = userAuthentication;
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<EventDateRequest, EventDate>();
            cfg.CreateMap<ParticipantRequest, Participant>()
                .ForMember(p => p.EventDates, opt => opt.MapFrom(p => p.EventDates));
        });
        _mapper = new Mapper(cfg);
    }

    public OperationResponse SaveParticipant(ParticipantRequest participant)
    {
        try
        {
            participant.Validate();
            var userId = _userAuthentication.GetUserInformation().UserId;
            var mapParticipant = _mapper.Map<Participant>(participant);
            mapParticipant.UserId = userId;
            _participantService.SaveParticipant(_mapper.Map<Participant>(participant));
            return OperationResponse.NoContent<OperationResponse>();
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OperationResponse.BadRequest<OperationResponse>($"Incorrect data transmitted: {ex.Message}");
        }
        catch (UniqueConstraintException e)
        {
            _logger.LogError(e, "Participant already exists");
            return OperationResponse.BadRequest<OperationResponse>();
        }
        catch(StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OperationResponse.InternalServerError<OperationResponse>($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OperationResponse.InternalServerError<OperationResponse>(ex.Message);
        }
    }
}