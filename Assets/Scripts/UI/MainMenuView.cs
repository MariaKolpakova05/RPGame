using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuView : MonoBehaviour
{
    [Header("Panels")]
    public GameObject MainMenuPanel;
    public GameObject SettingsPanel;

    [Header("Buttons")]
    public Button PlayButton;
    public Button SettingsButton;
    public Button BackButton;

    [Header("Settings")]
    public Slider VolumeSlider;

    [Header("Peaceful Mode")]
    public Toggle PeacefulModeToggle;         // ← переключатель "Мирный режим"
    public TextMeshProUGUI PeacefulModeLabel; // опционально: текст рядом
}