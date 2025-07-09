/// <summary>
/// Clase encargada de cargar los datos del juego desde el repositorio.
/// Utiliza el patrón Unit of Work para interactuar con el repositorio de juegos.
/// Esta clase es parte de la capa de aplicación y se encarga de la lógica de negocio relacionada con la carga del juego.

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
        return data;
    }
}