using UnityEngine;

public class RangoEnemigo2D : MonoBehaviour
{
    private IMeleeEnemy meleeEnemy;
    private IEnemyAnimator enemyAnimator;

    private void Start()
    {
        meleeEnemy = GetComponentInParent<IMeleeEnemy>();
        enemyAnimator = GetComponentInParent<IEnemyAnimator>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player") && meleeEnemy != null)
        {
            enemyAnimator?.PlayWalk(false);
            enemyAnimator?.PlayRun(false);
            enemyAnimator?.PlayAttack(true);
            meleeEnemy.Attack();
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}