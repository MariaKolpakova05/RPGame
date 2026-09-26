using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MainMenuView view;

    private void Start()
    {
        view.MainMenuPanel.SetActive(true);
        view.SettingsPanel.SetActive(false);

        view.PlayButton.onClick.AddListener(PlayGame);
        view.SettingsButton.onClick.AddListener(OpenSettings);
        view.BackButton.onClick.AddListener(CloseSettings);

        // Громкость
        view.VolumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        view.VolumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Мирный режим
        if (view.PeacefulModeToggle != null)
        {
            bool peaceful = PlayerPrefs.GetInt("PeacefulMode", 0) == 1;
            view.PeacefulModeToggle.isOn = peaceful;
            view.PeacefulModeToggle.onValueChanged.AddListener(OnPeacefulModeChanged);
            UpdatePeacefulLabel(peaceful);
        }
    }

    private void PlayGame() => SceneManager.LoadScene("GameScene");

    private void OpenSettings()
    {
        view.MainMenuPanel.SetActive(false);
        view.SettingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        view.SettingsPanel.SetActive(false);
        view.MainMenuPanel.SetActive(true);
    }

    private void OnVolumeChanged(float v)
    {
        PlayerPrefs.SetFloat("Volume", v);
        AudioListener.volume = v;
    }

    private void OnPeacefulModeChanged(bool isOn)
    {
        PlayerPrefs.SetInt("PeacefulMode", isOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdatePeacefulLabel(isOn);
        Debug.Log($"Режим изменён: {(isOn ? "Мирный" : "Обычный")}");
    }

    private void UpdatePeacefulLabel(bool isOn)
    {
        if (view.PeacefulModeLabel != null)
            view.PeacefulModeLabel.text = isOn ? "Мирный" : "Обычный";
    }
}