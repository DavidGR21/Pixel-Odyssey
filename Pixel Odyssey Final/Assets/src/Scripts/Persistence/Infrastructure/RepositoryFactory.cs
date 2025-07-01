/// <summary>
/// Clase encargada de crear instancias del repositorio de juegos.
/// Utiliza el patrón Factory para encapsular la creación del repositorio.
/// Esta clase es parte de la infraestructura y se encarga de proporcionar una instancia del repositorio de juegos.
/// Puede ser extendida para crear diferentes implementaciones del repositorio si es necesario.
/// </summary>
public static class RepositoryFactory
{
    public static IGameRepository Create()
    {
        return new FileGameRepository();
    }
}