using System;
//доступен отовсюду без создания объекта
public static class GameEvents
{
    public static Action PlayerDied;
    public static Action<int> ScoreChanged; //событие передает новый счет при его изменении
    public static Action<int> EnemyKilled; //событие передает количество убитых врагов при изменении
    public static Action BossAppeared;
    public static Action VictoryAchieved;

    public static Action<string> BossMessage; //реплики босса
    public static Action BossDefeated; //смерть босса
}