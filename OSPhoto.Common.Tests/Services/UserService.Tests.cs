using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Services;

namespace OSPhoto.Common.Tests.Services;

[TestFixture]
public class UserServiceTests
{
    private IUserService sut;  // sut = System Under Test

    private List<Tuple<string, string>> _users = [];

    [OneTimeSetUp]
    public void SetUp()
    {
        _users.Add(new Tuple<string, string>("User1", "Password1"));
        _users.Add(new Tuple<string, string>("User2", "Password2"));

        Environment.SetEnvironmentVariable("USERS", $"{string.Join(';', _users.Select(u => $"{u.Item1}={u.Item2}"))}");


        var dbContext = Utilities.GetInMemoryDbContext();
        var logger = new Logger<UserService>(new LoggerFactory());

        dbContext.Database.OpenConnection();
        dbContext.Database.Migrate();

        dbContext.SeedUsers($"{string.Join(';', _users.Select(u => $"{u.Item1}={u.Item2}"))}");

        sut = new UserService(dbContext, logger);
    }

    [Test]
    public async Task CanLoginAndGivenSessionIdIsValid()
    {
        var user = _users.First();
        var sessionId = await sut.LoginAsync(user.Item1, user.Item2);
        Assert.That(sessionId, Is.Not.Null & Is.Not.Empty);
        Assert.That(await sut.IsSessionIdValidAsync(sessionId));
    }

    [Test]
    public async Task RandomSessionIdsAreInvalid()
    {
        Assert.That(await sut.IsSessionIdValidAsync(Guid.NewGuid().ToString()), Is.False);
    }
}
