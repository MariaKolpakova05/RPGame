using UnityEngine;

//содержит характеристики каждого оружия
[System.Serializable]
public class WeaponData
{
    public WeaponType type;
    public float damage;
    public float range;
    public float cooldown;
    public GameObject visualPrefab;
    public Color effectColor = Color.white;
}