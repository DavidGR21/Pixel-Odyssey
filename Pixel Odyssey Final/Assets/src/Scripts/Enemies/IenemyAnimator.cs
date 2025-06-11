public interface IEnemyAnimator
{
    void PlayWalk(bool value);
    void PlayRun(bool value);
    void PlayAttack(bool value);
    void PlayHurt();
    void PlayDie();
    bool IsHurt();
}