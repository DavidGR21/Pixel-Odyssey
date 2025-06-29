public class SaveGame
{
    private readonly IUnitOfWork unitOfWork;
    public SaveGame(IUnitOfWork uow) => unitOfWork = uow;

    public void Execute(PlayerData data)
    {
        unitOfWork.GameRepository.Save(data);
        unitOfWork.Commit();
    }
}