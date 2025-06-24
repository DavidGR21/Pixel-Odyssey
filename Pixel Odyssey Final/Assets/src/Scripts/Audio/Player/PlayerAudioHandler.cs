using UnityEngine;

public class PlayerAudioHandler
{
    private AudioClip stepClip;
    private AudioClip jumpClip;
    private AudioClip damageClip;
    private AudioClip attackClip;
    private AudioClip dashClip;

    private bool isStepPlaying = false;

    public PlayerAudioHandler(AudioClip step, AudioClip jump, AudioClip damage, AudioClip attack, AudioClip dash)
    {
        stepClip = step;
        jumpClip = jump;
        damageClip = damage;
        attackClip = attack;
        dashClip = dash;
    }

    public void PlayStepSound(bool loop)
    {
        if (stepClip == null) return;

        if (loop)
        {
            if (!isStepPlaying)
            {
                AudioManager.Instance.PlayPlayerSound(stepClip, preventOverlap: true);
                isStepPlaying = true;
            }
        }
        else
        {
            AudioManager.Instance.PlayPlayerSound(stepClip);
            isStepPlaying = false;
        }
    }

    public void StopStepSound()
    {
        if (stepClip == null) return;

        AudioManager.Instance.StopPlayerSound(stepClip);
        isStepPlaying = false;
    }

    public void PlayJumpSound()
    {
        if (jumpClip != null)
            AudioManager.Instance.PlayPlayerSound(jumpClip);
    }

    public void PlayDamageSound()
    {
        if (damageClip != null)
            AudioManager.Instance.PlayPlayerSound(damageClip);
    }

    public void PlayAttackSound()
    {
        if (attackClip != null)
            AudioManager.Instance.PlayPlayerSound(attackClip);
    }

    public void PlayDashSound()
    {
        if (dashClip != null)
            AudioManager.Instance.PlayPlayerSound(dashClip);
    }
}
