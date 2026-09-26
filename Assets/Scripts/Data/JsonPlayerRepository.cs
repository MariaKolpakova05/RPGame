using System.IO;
using UnityEngine;

public class JsonPlayerRepository : IPlayerRepository
{
    private readonly string _path;

    public JsonPlayerRepository()
    {
        _path = Path.Combine(Application.persistentDataPath, "player.json");
    }

    public void Save(PlayerData data)
    {
        File.WriteAllText(_path, JsonUtility.ToJson(data, true));
    }

    public PlayerData Load()
    {
        if (!File.Exists(_path)) return null;
        return JsonUtility.FromJson<PlayerData>(File.ReadAllText(_path));
    }

    public bool HasSavedData() => File.Exists(_path);

    public void Clear()
    {
        if (File.Exists(_path)) File.Delete(_path);
    }
}