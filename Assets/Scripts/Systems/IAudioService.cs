public interface IAudioService
{
    void PlayBackgroundMusic();
    void PlayVictoryMusic();
    void PlayDeathSound();
    void PlayAttackSound();
    void PlayMagicSound();
    void SetVolume(float volume);
    float GetVolume();
}