using UnityEngine;

public class AttackBehavior : IEnemyBehavior
{
    public void Execute(Enemy enemy)
    {
        if (enemy is IMeleeEnemy meleeEnemy)
        {
            meleeEnemy.Attack();
        }
    }
}