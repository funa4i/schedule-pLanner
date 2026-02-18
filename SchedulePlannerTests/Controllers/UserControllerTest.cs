using System.Net;
using SchedulePlannerBack.Domain.Views;

namespace SchedulePlannerTests.Controllers;

[TestFixture]
public class UserControllerTest : BaseWebApiControllerTest
{
    [Test]
    public async Task Get_GetUserProfile_WhenAuthorized_ReturnsOk()
    {
        var response = await HttpClient.GetAsync("/User");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await GetModelFromResponseAsync<UserView>(response);

        Assert.That(data, Is.Not.Null);
    }

    [Test]
    public async Task Get_GetUserProfile_WhenUnauthorized_ReturnsUnauthorized()
    {
        // HttpClient.DefaultRequestHeaders.Remove("Authorization");
        var response = await HttpClient.GetAsync("/User");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
