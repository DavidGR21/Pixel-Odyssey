using UnityEngine;

public class PatrolBehavior : IEnemyBehavior
{
    public void Execute(Enemy enemy)
    {
        if (enemy is IMeleeEnemy)
        {
            var animator = enemy.GetAnimator();
            enemy.chronometer += Time.deltaTime;
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
                enemy.direction = enemy.direction == 0 ? 1 : 0;
                enemy.transform.rotation = Quaternion.Euler(0, enemy.direction == 0 ? 0 : 180, 0);
                enemy.chronometer = 0;
                animator.PlayWalk(false);
                return;
            }
            // --- FIN debug ---

            if (enemy.chronometer >= 4)
            {
                enemy.rutine = Random.Range(0, 2);
                enemy.chronometer = 0;
            }
            switch (enemy.rutine)
            {
                case 0:
                    animator.PlayWalk(false);
                    break;
                case 1:
                    enemy.direction = Random.Range(0, 2);
                    enemy.rutine++;
                    break;
                case 2:
                    enemy.transform.rotation = Quaternion.Euler(0, enemy.direction == 0 ? 0 : 180, 0);
                    enemy.transform.Translate(Vector3.right * enemy.speedWalk * Time.deltaTime);
                    animator.PlayWalk(true);
                    break;
            }
        }
    }
}