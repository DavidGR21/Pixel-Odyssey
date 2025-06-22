
public class LoadGame
{
    private readonly IGameRepository repository;

    public LoadGame(IGameRepository repo)
    {
        repository = repo;
    }

    public PlayerData Execute()
    {
        return repository.Load();
    }
}