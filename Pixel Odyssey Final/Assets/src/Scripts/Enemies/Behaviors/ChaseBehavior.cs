using UnityEngine;
/// <summary>
/// Representa el comportamiento de persecución para un enemigo que implementa <see cref="IMeleeEnemy"/>.
/// Cuando se ejecuta, el enemigo girará para mirar a su objetivo, reproducirá la animación de correr
/// y se moverá hacia el objetivo a velocidad de carrera.
/// </summary>
/// <param name="enemy">
/// Instancia de <see cref="Enemy"/> que ejecuta el comportamiento de persecución. Debe implementar <see cref="IMeleeEnemy"/>.
/// </param>
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
