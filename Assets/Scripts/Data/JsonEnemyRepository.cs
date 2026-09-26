using System.IO;
using UnityEngine;

[System.Serializable]
public class EnemyPositionsWrapper
{
    public float[] positions;
}

public class JsonEnemyRepository : IEnemyRepository
{
    private readonly string _path;

    public JsonEnemyRepository()
    {
        _path = Path.Combine(Application.persistentDataPath, "enemies.json");
    }

    public void Save(float[] positions)
    {
        var wrapper = new EnemyPositionsWrapper { positions = positions };
        File.WriteAllText(_path, JsonUtility.ToJson(wrapper, true));
    }

    public float[] Load()
    {
        if (!File.Exists(_path)) return null;
        var wrapper = JsonUtility.FromJson<EnemyPositionsWrapper>(File.ReadAllText(_path));
        return wrapper?.positions;
    }

    public bool HasSavedData() => File.Exists(_path);

    public void Clear()
    {
        if (File.Exists(_path)) File.Delete(_path);
    }
}