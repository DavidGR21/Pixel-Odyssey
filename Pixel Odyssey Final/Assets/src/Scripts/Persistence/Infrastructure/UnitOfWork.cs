/// <summary>
/// Unit of Work mejorado que permite trabajar con diferentes tipos de repositorios.
/// Mantiene el principio de responsabilidad única y permite transacciones.
/// Implementa IDisposable para manejo correcto de recursos.
/// </summary>
public class UnitOfWork : IUnitOfWork, System.IDisposable
{
    private readonly IGameRepository _gameRepository;
    private bool _committed = false;
    private bool _disposed = false;

    public UnitOfWork(RepositoryFactory.RepositoryType repositoryType = RepositoryFactory.RepositoryType.File, 
                      DatabaseConfig databaseConfig = null)
    {
        _gameRepository = RepositoryFactory.Create(repositoryType, databaseConfig);
    }

    public IGameRepository GameRepository => _gameRepository;

    public void Commit()
    {
        if (!_disposed)
        {
            _committed = true;
            // Aquí podrías implementar lógica de transacciones si fuera necesario
        }
    }

    public void Rollback()
    {
        if (!_disposed)
        {
            _committed = false;
            // Aquí podrías implementar lógica de rollback si fuera necesario
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_gameRepository is System.IDisposable disposableRepo)
            {
                disposableRepo.Dispose();
            }
            _disposed = true;
        }
    }
}