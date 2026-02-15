namespace SchedulePlannerBack.Domain.Views;

public class UserView
{
    public UserView(long userId, string login)
    {
        UserId = userId;
        Login = login;
    }

    public long UserId {get; set;}
    public string Login {get; set;}
}