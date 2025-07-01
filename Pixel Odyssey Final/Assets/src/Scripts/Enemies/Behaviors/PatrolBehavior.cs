using UnityEngine;
/// <summary>
/// Clase que implementa el comportamiento de patrullaje para enemigos que implementan <see cref="IMeleeEnemy"/>.
/// El enemigo patrullará entre dos puntos, cambiará de dirección al encontrar un obstáculo
/// y alternará entre rutinas de espera, cambio de dirección y movimiento.
/// </summary>
public class PatrolBehavior : IEnemyBehavior
{
    public void Execute(Enemy enemy)
    {
        if (enemy is IMeleeEnemy)
        {

            var animator = enemy.GetAnimator();
            enemy.chronometer += Time.deltaTime;
            animator.PlayRun(false);

            // --- Detección de suelo y pared 
            bool isGroundAhead = true;
            bool isWallAhead = false;
            //se comprueba que el enemigo tenga los checks asignados
            if (enemy.groundCheck != null && enemy.wallCheck != null)
            {
                // Se comprueba si hay suelo y pared delante del enemigo
                isGroundAhead = Physics2D.OverlapCircle(enemy.groundCheck.position, enemy.checkRadius, enemy.groundLayer);
                isWallAhead = Physics2D.OverlapCircle(enemy.wallCheck.position, enemy.checkRadius, enemy.groundLayer);

            }

            // --- FIN detección de suelo y pared ---

            if (!isGroundAhead || isWallAhead)
            {
                // Cambia de dirección si no hay suelo o hay una pared
                enemy.direction = enemy.direction == 0 ? 1 : 0;
                enemy.transform.rotation = Quaternion.Euler(0, enemy.direction == 0 ? 0 : 180, 0);
                enemy.chronometer = 0;
                animator.PlayWalk(false);
                return;
            }

            if (enemy.chronometer >= 4)
            {
                // Cambia de rutinas cada 4 segundos
                enemy.rutine = Random.Range(0, 2);
                enemy.chronometer = 0;
            }
            // Rutinas de movimiento
            switch (enemy.rutine)
            {
                // Rutina 0: Espera
                case 0:
                    animator.PlayWalk(false);
                    break;
                case 1:
                    // Rutina 1: Cambio de dirección
                    enemy.direction = Random.Range(0, 2);
                    enemy.rutine++;
                    break;
                case 2:
                    // Rutina 2: Movimiento
                    enemy.transform.rotation = Quaternion.Euler(0, enemy.direction == 0 ? 0 : 180, 0);
                    enemy.transform.Translate(Vector3.right * enemy.speedWalk * Time.deltaTime);
                    animator.PlayWalk(true);
                    break;
            }
        }
    }
}