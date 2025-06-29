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
        // Aquí podrías hacer flush, guardar logs, etc.
        _committed = true;
        // Si tuvieras varios repositorios, aquí guardarías todos juntos
    }

    public void Rollback()
    {
        // Si tuvieras operaciones en memoria, aquí las desharías
        _committed = false;
    }
}