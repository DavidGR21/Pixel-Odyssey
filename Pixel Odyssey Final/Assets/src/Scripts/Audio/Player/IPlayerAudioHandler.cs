public interface IPlayerAudioHandler
{
    void PlayStepSound(bool loop);
    void StopStepSound();
    void PlayJumpSound();
    void PlayDamageSound();
    void PlayAttackSound();
    void PlayDashSound();
}
