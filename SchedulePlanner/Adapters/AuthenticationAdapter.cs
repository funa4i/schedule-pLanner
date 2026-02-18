using AutoMapper;
using EntityFramework.Exceptions.Common;
using SchedulePlannerBack.Controller;
using SchedulePlannerBack.Domain.Bindings;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Views;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Service;

namespace SchedulePlannerBack.Adapters;

public class AuthenticationAdapter
{
    private readonly AuthenticationService _authenticationService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    
    public AuthenticationAdapter(AuthenticationService authenticationService, ILogger logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserRequest, User>();
        });
        _mapper = new Mapper(cfg);
    }

    public OperationResponse CreateUser(UserRequest user)
    {
        try
        {
            user.Validate();
            _authenticationService.RegisterUser(_mapper.Map<User>(user));
            return OperationResponse.NoContent<OperationResponse>();
        }
        catch (UniqueConstraintException e)
        {
            _logger.LogError(e, "User already exists");
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

    public OperationResponse ValidateUser(UserRequest user)
    {
        try
        {
            user.Validate();
            return OperationResponse
                .Ok<OperationResponse, JwtResponse>(new JwtResponse(_authenticationService
                    .ValidateUser(_mapper.Map<User>(user))));
            
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OperationResponse.BadRequest<OperationResponse>($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException e)
        {
            _logger.LogError(e, "User not found");
            return OperationResponse.NotFound<OperationResponse>();
        }
        catch (AuthenticationException e)
        {
            _logger.LogError(e, "Authentication error");
            return OperationResponse.BadRequest<OperationResponse>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception");
            return OperationResponse.InternalServerError<OperationResponse>
                ($"Error while working with data storage: {e.InnerException!.Message}");
        }
       
    }
    
}