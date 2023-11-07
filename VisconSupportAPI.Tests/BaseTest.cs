namespace VisconSupportAPI.Tests;

public class BaseTest
{
    protected readonly IConfiguration Config;
    protected readonly HttpClient HttpClient;
    protected readonly ITestOutputHelper TestOutputHelper;
    
    public BaseTest(ITestOutputHelper testOutputHelper)
    {
        this.Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testconfig.json")
            .Build();
        
        WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();
        this.HttpClient = factory.CreateDefaultClient();

        this.TestOutputHelper = testOutputHelper;
    }
    
    public async Task SetAccount(AccountType accountType)
    {
        UserCredentials credentials = accountType switch
        {
            AccountType.User => new UserCredentials()
            {
                Username = Config["LoginData:Customer:Username"] ?? "",
                Password = Config["LoginData:Customer:Password"] ?? ""
            },
            AccountType.Helpdesk => new UserCredentials()
            {
                Username = Config["LoginData:Employee:Username"] ?? "",
                Password = Config["LoginData:Employee:Password"] ?? ""
            },
            AccountType.Admin => new UserCredentials()
            {
                Username = Config["LoginData:Admin:Username"] ?? "",
                Password = Config["LoginData:Admin:Password"] ?? ""
            },
            _ => throw new ArgumentOutOfRangeException(nameof(accountType), accountType, null)
        };

        HttpResponseMessage res = await HttpClient.PostAsync(
            "/api/login",
            new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json"));
        
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        
        string data = await res.Content.ReadAsStringAsync();

        string? token = JsonConvert.DeserializeAnonymousType(data, new { Token = "" })?.Token;
        
        Assert.NotNull(token);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}