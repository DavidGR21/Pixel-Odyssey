public interface IUnitOfWork
{
    void Commit();
    void Rollback();
    IGameRepository GameRepository { get; }
}