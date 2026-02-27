using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _gunSfxSource;
    // bgm Source 

    [Space(10)]
    [Header("Audio Clip")]
    [SerializeField] private AudioClip _glockSFX;
    [SerializeField] private AudioClip _mp7SFX;
    [SerializeField] private AudioClip _m700SFX;
    [SerializeField] private AudioClip _reloadSFX;

    public void PlayGunSFX(uint gunId)
    {
        _gunSfxSource.pitch = 1f;

        if (gunId == GunId.GlockId)
        {
            _gunSfxSource.PlayOneShot(_glockSFX);
        }
        else if (gunId == GunId.Mp7Id)
        {
            _gunSfxSource.PlayOneShot(_mp7SFX);
        }
        else if (gunId == GunId.M700Id)
        {
            _gunSfxSource.PlayOneShot(_m700SFX);
        }
    }

    public void PlayReloadSFX(uint gunId, float reloadTime)
    {
        _gunSfxSource.pitch = _reloadSFX.length / reloadTime;
        _gunSfxSource.PlayOneShot(_reloadSFX);
    }

}
