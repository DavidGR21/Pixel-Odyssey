using UnityEngine;

public class ChaseBehavior : IEnemyBehavior
{
    public void Execute(Enemy enemy)
    {
        if (enemy is IMeleeEnemy)
        {
            var animator = enemy.GetAnimator();
            if (enemy.target == null) return;
            enemy.transform.rotation = Quaternion.Euler(0, enemy.transform.position.x < enemy.target.transform.position.x ? 0 : 180, 0);
            animator.PlayWalk(false);
            animator.PlayRun(true);
            animator.PlayAttack(false);
            enemy.transform.Translate(Vector3.right * enemy.speedRun * Time.deltaTime);
        }
    }
}