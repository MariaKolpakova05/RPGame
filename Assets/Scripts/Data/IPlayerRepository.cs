public interface IPlayerRepository
{
    void Save(PlayerData data);
    PlayerData Load();
    bool HasSavedData();
    void Clear();
}