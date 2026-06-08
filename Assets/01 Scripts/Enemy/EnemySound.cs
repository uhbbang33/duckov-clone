using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [SerializeField] private AudioSource _footStepAudioSource;
    [SerializeField] private AudioSource _gunAudioSource;
    [SerializeField] private AudioSource _detectAudioSource;

    private SoundManager _soundManager;

    private void Start() => _soundManager = SoundManager.Instance;

    public void PlayFire(uint id) => _soundManager.PlayGunSFX(id, _gunAudioSource);
    public void PlayReload(bool isStart) => _soundManager.PlayReloadSFX(isStart, _gunAudioSource);
    public void PlayFootStep(bool isRun) => _soundManager.PlayFootStepSFX(isRun, _footStepAudioSource);
    public void StopFootStep() => _footStepAudioSource.Stop();
    public void PlayDetect() => _soundManager.PlaySFXOneShot(SFXName.Quack, _detectAudioSource);
}
