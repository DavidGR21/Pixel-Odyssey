using UnityEngine;
using UnityEditor.Animations;
using System;
using System.Collections;
/// <summary>
///  Clase encargada de controlar las animaciones de los enemigos.
/// Esta clase utiliza un Animator para reproducir animaciones de caminar, correr, atacar, recibir daño y morir.
/// También permite la inyección de un IEnemyAnimator personalizado para manejar animaciones específicas.
/// </summary>
public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;
    private IEnemyAnimator enemyAnimator;

    public void Initialize()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
        }
    }

    public void InjectAnimator(IEnemyAnimator animator)
    {
        this.enemyAnimator = animator;
    }

    public IEnemyAnimator GetAnimator()
    {
        return enemyAnimator ?? GetComponent<IEnemyAnimator>();
    }

    public void PlayWalk(bool state)
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.PlayWalk(state);
        }
        else if (animator != null)
        {
            animator.SetBool("walk", state);
        }
    }

    public void PlayRun(bool state)
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.PlayRun(state);
        }
        else if (animator != null)
        {
            animator.SetBool("run", state);
        }
    }

    public void PlayAttack(bool state)
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.PlayAttack(state);
        }
        else if (animator != null)
        {
            animator.SetBool("attack", state);
        }
    }

    public void SetHurtAnimation(bool state)
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.PlayHurt(state);
        }
        else if (animator != null)
        {
            animator.SetBool("hurt", state);
        }
    }

    public void SetDeathAnimation()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.PlayDeath();
        }
        else if (animator != null)
        {
            animator.SetTrigger("dead");
        }
    }

    public void ResetHurtAnimation(Action onComplete)
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.ResetHurt(() => StartCoroutine(ResetHurtWithDuration(0.3f, onComplete)));
        }
        else
        {
            StartCoroutine(ResetHurtWithDuration(GetHurtAnimationDuration(), onComplete));
        }
    }

    private float GetHurtAnimationDuration()
    {
        if (animator == null)
        {
            return 0.3f; // Default duration if animator is missing
        }

        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            return 0.3f; // Default duration if controller is missing
        }

        foreach (var layer in animatorController.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.name == "hurt")
                {
                    AnimationClip clip = state.state.motion as AnimationClip;
                    if (clip != null)
                    {
                        float duration = clip.length / (state.state.speed != 0 ? state.state.speed : 1);
                        return duration;
                    }
                }
            }
        }

        return 0.3f; // Default duration if hurt animation is not found
    }

    private IEnumerator ResetHurtWithDuration(float duration, Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        if (animator != null)
        {
            SetHurtAnimation(false);
            onComplete?.Invoke();
        }
    }
}