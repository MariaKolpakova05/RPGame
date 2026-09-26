//хранит данные UI отдельно от отображения
public class UIModel
{
    public float PlayerHealth { get; set; }
    public float PlayerMaxHealth { get; set; }
    public float PlayerMana { get; set; }
    public float PlayerMaxMana { get; set; }
    public float MagicCooldownProgress { get; set; }
    public int Score { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsPaused { get; set; }
}