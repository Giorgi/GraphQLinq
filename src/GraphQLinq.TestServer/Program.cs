namespace TestServer;

public class Program
{
    public static CancellationTokenSource CancellationTokenSource = new();
    
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(10_000));

        builder.Services.AddGraphQLServer()
            .AddQueryType<Query>()
            .AddTypeExtension<UserGraphQLExtensions>();

        var app = builder.Build();

        app.MapGraphQL();

        await app.RunAsync(CancellationTokenSource.Token);
    }
}

public class Query
{

}

[ExtendObjectType(typeof(Query))]
public class UserGraphQLExtensions
{
    public User GetUserTemporaryFixForNullable(int? id)
    {
        return new User
        {
            FirstName = "Jon",
            LastName = "Smith"
        };
    }
    
    public User GetUser(int id)
    {
        return new User
        {
            FirstName = "Jon",
            LastName = "Smith"
        };
    }

    public User GetFailUser()
    {
        throw new GraphQLException(new Error("Property fails", "FAIL_PROPERTY"));
    }
}

public class User
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}