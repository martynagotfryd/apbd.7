namespace apbd._7.Repositories;

public class Repository : IRepository
{
    private readonly IConfiguration _configuration;

    public Repository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    
}