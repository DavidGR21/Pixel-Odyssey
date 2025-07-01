/// <summary>
/// Clase que implementa un patrón Unit of Work para manejar transacciones y acceso a repositorios.
/// Esta clase es parte de la infraestructura y se encarga de proporcionar una instancia del repositorio de juegos.
/// Permite realizar operaciones de commit y rollback, encapsulando la lógica de persistencia.
/// Puede ser extendida para manejar múltiples repositorios si es necesario.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly FileGameRepository _gameRepository;
    private bool _committed = false;

    public UnitOfWork()
    {
        _gameRepository = new FileGameRepository();
    }

    public IGameRepository GameRepository => _gameRepository;

    public void Commit()
    {
        _committed = true;
    }

    public void Rollback()
    {
        _committed = false;
    }
}