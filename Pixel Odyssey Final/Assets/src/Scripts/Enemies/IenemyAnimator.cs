public interface IEnemyAnimator
{
    void PlayWalk(bool value);
    void PlayRun(bool value);
    void PlayAttack(bool value);
    void PlayHurt(bool value); // <-- Cambia a bool
    void PlayDie();
    bool IsHurt();//
}