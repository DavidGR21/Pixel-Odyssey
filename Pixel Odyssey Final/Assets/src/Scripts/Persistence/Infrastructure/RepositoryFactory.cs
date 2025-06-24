
public static class RepositoryFactory
{
    public static IGameRepository Create()
    {
        return new FileGameRepository();
        // O puedes decidir según plataforma/configuración
    }
}