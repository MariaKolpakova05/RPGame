//интерфейс для объектов, которые могут наносить урон
public interface IDamageDealer
{
    float GetDamage(); //сила атк
    DamageType GetDamageType(); //тип урона физа или маг
}