/// <summary>
/// Interfaz que define las operaciones de un patrón Unit of Work.
/// Permite manejar transacciones y acceder al repositorio de juegos.
/// Esta interfaz debe ser implementada por cualquier clase que maneje la persistencia de datos del juego.
/// </summary>
public interface IUnitOfWork
{
    void Commit();
    void Rollback();
    IGameRepository GameRepository { get; }
}