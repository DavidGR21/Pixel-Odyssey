
public class SaveGame
{
    private readonly IGameRepository repository;
    public SaveGame(IGameRepository repo) => repository = repo;
    public void Execute(PlayerData data) => repository.Save(data);
}