using UnityEngine;

public class PeacefulModeController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TogglePeaceful();
    }

    public void TogglePeaceful()
    {
        bool newValue = !EnemyBase.PeacefulMode;
        EnemyBase.PeacefulMode = newValue;
        BossEnemy.PeacefulMode = newValue;

        // Синхронизируем с настройкой в меню
        PlayerPrefs.SetInt("PeacefulMode", newValue ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"Peaceful mode: {newValue}");
    }
}