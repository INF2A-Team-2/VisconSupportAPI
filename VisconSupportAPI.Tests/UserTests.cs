namespace VisconSupportAPI.Tests;

public class UserTests : BaseTest
{
    public UserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        
    }

    [Fact]
    public async Task TestGetUser()
    {
        await SetAccount(AccountType.Admin);

        string? userIdS = Config["LoginData:Admin:Id"];
        
        Assert.NotNull(userIdS);

        int userId = int.Parse(userIdS);

        HttpResponseMessage res = await HttpClient.GetAsync($"/api/users/{userId}");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        
        string data = await res.Content.ReadAsStringAsync();

        User? user = JsonConvert.DeserializeObject<User>(data);
        
        Assert.NotNull(user);
        
        string? username = Config["LoginData:Admin:Username"];
        
        Assert.Equal(user.Id, userId);
        Assert.Equal(user.Username, username);
    }
}