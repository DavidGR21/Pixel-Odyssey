using UnityEngine;

public class enemy : MonoBehaviour
{
    [SerializeField] private float life;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void takeDamage(float damage)
    {
        life -= damage;
        if (life <= 0) 
        {
            Dead();
        }
    }
    // Update is called once per frame
    private void Dead() 
    {
        animator.SetTrigger("Dead");
    }
}
