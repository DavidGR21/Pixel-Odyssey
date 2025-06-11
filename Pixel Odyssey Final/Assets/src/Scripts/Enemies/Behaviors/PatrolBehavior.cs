using UnityEngine;

public class PatrolBehavior : IEnemyBehavior
{
    public void Execute(Enemy enemy)
    {
        if (enemy is IMeleeEnemy)
        {
            var animator = enemy.GetAnimator();
            enemy.cronometro += Time.deltaTime;
            animator.PlayRun(false);
            if (enemy.cronometro >= 4)
            {
                enemy.rutina = Random.Range(0, 2);
                enemy.cronometro = 0;
            }
            switch (enemy.rutina)
            {
                case 0:
                    animator.PlayWalk(false);
                    break;
                case 1:
                    enemy.direccion = Random.Range(0, 2);
                    enemy.rutina++;
                    break;
                case 2:
                    enemy.transform.rotation = Quaternion.Euler(0, enemy.direccion == 0 ? 0 : 180, 0);
                    enemy.transform.Translate(Vector3.right * enemy.speedWalk * Time.deltaTime);
                    animator.PlayWalk(true);
                    break;
            }
        }
    }
}