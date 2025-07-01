/// <summary>
/// Clase encargada de guardar los datos del juego en el repositorio.
/// Utiliza el patrón Unit of Work para interactuar con el repositorio de juegos.
/// Esta clase es parte de la capa de aplicación y se encarga de la lógica de negocio relacionada con el guardado del juego.
/// </summary>
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