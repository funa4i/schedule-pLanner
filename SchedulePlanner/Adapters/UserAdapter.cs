using AutoMapper;
using SchedulePlannerBack.Controller;
using SchedulePlannerBack.Domain.Bindings;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Views;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Interfaces.Application;
using SchedulePlannerBack.Service;
using SchedulePlannerBack.Util;

namespace SchedulePlannerBack.Adapters;

public class UserAdapter
{
    private readonly IUserService _userService;
    private readonly ILogger _logger;
    private readonly UserAuthenticationProvider _userAuthenticationProvider;
    private readonly IMapper _mapper;

    public UserAdapter(IUserService userService, ILogger logger, UserAuthenticationProvider userAuthenticationProvider)
    {
        _userService = userService;
        _logger = logger;
        _userAuthenticationProvider = userAuthenticationProvider;
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserView>()
                .ForMember(x => x.UserId,
                    x => x.MapFrom(src => src.Id));
        });
        _mapper = new Mapper(cfg);
    }

    public OperationResponse GetUserProfileWithAuthentication()
    {
        try
        {
            var userId = _userAuthenticationProvider.GetUserInformation().UserId;
            var userView = _userService.GetUserById(userId ?? throw new AuthenticationException());
            return OperationResponse.Ok<OperationResponse, UserView>(_mapper.Map<UserView>(userView));
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