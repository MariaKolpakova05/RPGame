using UnityEngine;
using System;

public class ManaComponent : MonoBehaviour
{
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float regenPerSecond = 5f;
    private float _currentMana;

    public event Action<float, float> OnManaChanged;

    public float CurrentMana => _currentMana;
    public float MaxMana => maxMana;

    private void Start()
    {
        _currentMana = maxMana;
        OnManaChanged?.Invoke(_currentMana, maxMana);
    }

    private void Update()
    {
        if (_currentMana < maxMana)
        {
            _currentMana = Mathf.Min(maxMana, _currentMana + regenPerSecond * Time.deltaTime);
            OnManaChanged?.Invoke(_currentMana, maxMana);
        }
    }

    public bool TrySpend(float amount)
    {
        if (_currentMana < amount) return false;
        _currentMana -= amount;
        OnManaChanged?.Invoke(_currentMana, maxMana);
        return true;
    }

    public void SetMana(float value)
    {
        _currentMana = Mathf.Clamp(value, 0, maxMana);
        OnManaChanged?.Invoke(_currentMana, maxMana);
    }
}