public class LoadGame
{
    private readonly IGameRepository repository;

    public LoadGame(IGameRepository repo)
    {
        repository = repo;
    }

    // Nuevo método para perfiles
    public PlayerData Execute(int profileId)
    {
        return repository.Load(profileId);
    }

    // Método antiguo (sin perfil)
    public PlayerData Execute()
    {
        return repository.Load();
    }
}