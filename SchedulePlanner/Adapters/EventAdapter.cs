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

public class EventAdapter
{
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly EventService _eventService;
    private readonly UserAuthenticationProvider _userAuthentication;

    public EventAdapter(ILogger logger, EventService eventService,
        UserAuthenticationProvider userAuthentication)
    {
        _logger = logger;
        _eventService = eventService;
        _userAuthentication = userAuthentication;
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<EventDateRequest, EventDate>();
            cfg.CreateMap<EventRequest, Event>();
            cfg.CreateMap<Event, EventView>()
                .ForMember(x => x.AuthorName,
                    src => src.MapFrom(x => x.User!.Login));
            cfg.CreateMap<ParticipantRequest, Participant>()
                .ForMember(p => p.EventDates, opt => opt.MapFrom(p => p.EventDates));
        });
        _mapper = new Mapper(cfg);
    }

    public OperationResponse GetEventWithLink(string eventLink)
    {
        try
        {
            return OperationResponse
                .Ok<OperationResponse, EventView>(_mapper
                    .Map<EventView>(_eventService.GetEvent(eventLink)));
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OperationResponse.InternalServerError<OperationResponse>(
                $"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OperationResponse.InternalServerError<OperationResponse>(ex.Message);
        }
    }

    public OperationResponse CreateEvent(EventRequest newEvent)
    {
        try
        {
            newEvent.Validate();
            var dates = newEvent.EventDateRequests.Select(d => _mapper.Map<EventDate>(d)).ToList();
            var mapEvent = _mapper.Map<Event>(newEvent);
            var userId = _userAuthentication.GetUserInformation().UserId!.Value;
            var result = _eventService.CreateEvent(mapEvent, dates, userId);
            return OperationResponse.Ok<OperationResponse, EventView>(_mapper.Map<EventView>(result));

        }
        catch (UniqueConstraintException e)
        {
            _logger.LogError(e, "Event already exists");
            return OperationResponse.BadRequest<OperationResponse>();
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OperationResponse.BadRequest<OperationResponse>($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OperationResponse.InternalServerError<OperationResponse>(
                $"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OperationResponse.InternalServerError<OperationResponse>(ex.Message);
        }
    }

    public OperationResponse GetAllEventsWithUserId()
    {
        try
        {
            var userId = _userAuthentication.GetUserInformation().UserId!.Value;
            return OperationResponse.Ok<OperationResponse, 
                List<EventView>>([.. _eventService.GetAllEventsWithUserId(userId)
                .Select(e => _mapper.Map<EventView>(e)).ToList()]);
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

    public OperationResponse ResultEventTime(string eventLink)
    {
        try
        {
            var userId = _userAuthentication.GetUserInformation().UserId!.Value;
            var result = _eventService.ResultEventTime(eventLink, userId);
            return OperationResponse.Ok<OperationResponse, EventView>(_mapper.Map<EventView>(result));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OperationResponse.BadRequest<OperationResponse>($"Incorrect data transmitted: {ex.Message}");
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