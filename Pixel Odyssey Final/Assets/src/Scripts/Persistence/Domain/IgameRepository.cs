public interface IGameRepository
{
    void Save(PlayerData data);
    PlayerData Load();
        PlayerData Load(int profileId);

}