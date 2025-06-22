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

            // --- Detección de suelo y pared con debug ---
            bool isGroundAhead = true;
            bool isWallAhead = false;

            if (enemy.groundCheck != null && enemy.wallCheck != null)
            {
                isGroundAhead = Physics2D.OverlapCircle(enemy.groundCheck.position, enemy.checkRadius, enemy.groundLayer);
                isWallAhead = Physics2D.OverlapCircle(enemy.wallCheck.position, enemy.checkRadius, enemy.groundLayer);

                Debug.Log($"[{enemy.name}] isGroundAhead: {isGroundAhead}, isWallAhead: {isWallAhead}, groundCheckPos: {enemy.groundCheck.position}, wallCheckPos: {enemy.wallCheck.position}");
            }
            else
            {
                Debug.LogWarning($"[{enemy.name}] groundCheck o wallCheck no asignados.");
            }

            if (!isGroundAhead || isWallAhead)
            {
                Debug.Log($"[{enemy.name}] ¡Girando! Motivo: {(isWallAhead ? "Pared detectada" : "No hay suelo adelante")}");
                // Cambia de dirección si no hay suelo o hay una pared
                enemy.direccion = enemy.direccion == 0 ? 1 : 0;
                enemy.transform.rotation = Quaternion.Euler(0, enemy.direccion == 0 ? 0 : 180, 0);
                enemy.cronometro = 0;
                animator.PlayWalk(false);
                return;
            }
            // --- FIN debug ---

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