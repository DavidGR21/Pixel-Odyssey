/// <summary>
/// Factory mejorado que permite elegir entre diferentes implementaciones de repositorio.
/// Ahora incluye soporte para SQLite como opción principal.
/// Mantiene los principios SOLID y permite fácil extensión.
/// </summary>
public static class RepositoryFactory
{
    public enum RepositoryType
    {
        File,
        SQLite,
        MySQL
    }

    public static IGameRepository Create(RepositoryType type = RepositoryType.SQLite, DatabaseConfig config = null)
    {
        switch (type)
        {
            case RepositoryType.File:
                return new FileGameRepository();
            
            case RepositoryType.SQLite:
                string databaseName = config?.databaseName ?? "PixelOdyssey.db";
                return new SQLiteGameRepository(databaseName);
            
            case RepositoryType.MySQL:
                if (config == null)
                {
                    throw new System.ArgumentException("DatabaseConfig es requerido para MySQL repository");
                }
                // Implementación futura de MySQL
                throw new System.NotImplementedException("MySQL repository no implementado aún");
            
            default:
                return new SQLiteGameRepository();
        }
    }
}