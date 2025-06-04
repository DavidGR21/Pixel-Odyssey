using UnityEngine;

public class RangoEnemigo2D : MonoBehaviour
{
    public Animator animator;
    private IMeleeEnemy meleeEnemy;

    private void Start()
    {
        meleeEnemy = GetComponentInParent<IMeleeEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player") && meleeEnemy != null)
        {
            animator.SetBool("walk", false);
            animator.SetBool("run", false);
            animator.SetBool("attack", true);
            meleeEnemy.Attack();
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}   