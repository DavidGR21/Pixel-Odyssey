public class LoadGame
{
    private readonly IUnitOfWork unitOfWork;

    public LoadGame(IUnitOfWork uow)
    {
        unitOfWork = uow;
    }

    public PlayerData Execute(int profileId)
    {
        var data = unitOfWork.GameRepository.Load(profileId);
        // Si necesitas, puedes llamar a unitOfWork.Commit() aquí, pero para lecturas normalmente no es necesario.
        return data;
    }
}