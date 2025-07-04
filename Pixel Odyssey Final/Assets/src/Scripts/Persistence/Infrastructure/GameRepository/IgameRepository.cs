/// <summary>
/// Interfaz que define las operaciones de un repositorio de juegos.
/// Permite guardar y cargar datos del jugador.
/// Esta interfaz debe ser implementada por cualquier clase que maneje la persistencia de datos del juego.
/// </summary>
public interface IGameRepository
{
    void Save(PlayerData data);
    PlayerData Load(int profileId);

}