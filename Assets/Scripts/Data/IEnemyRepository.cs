public interface IEnemyRepository
{
    void Save(float[] positions);
    float[] Load();
    bool HasSavedData();
    void Clear();
}