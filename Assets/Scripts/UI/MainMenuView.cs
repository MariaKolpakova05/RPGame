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
    public Button QuitButton;

    [Header("Settings")]
    public Slider VolumeSlider;

    [Header("Peaceful Mode")]
    public Toggle PeacefulModeToggle;
    public TextMeshProUGUI PeacefulModeLabel;
}