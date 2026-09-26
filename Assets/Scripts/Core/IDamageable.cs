using UnityEngine;

//определение типа урона
public enum DamageType
{
    Physical,
    Magical
}
//интерфейс для всех объектов, которок могут получать урон
public interface IDamageable
{
    void TakeDamage(float amount, DamageType type); //метод получения урона
    bool IsAlive { get; } //свойство "если жив"
    Transform Transform { get; } //свойство позиции объекта
}