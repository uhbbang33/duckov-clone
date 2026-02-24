using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _sfxSource;
    // bgm Source 

    [Space(10)]
    [Header("Audio Clip")]
    [SerializeField] private AudioClip _glockSFX;
    [SerializeField] private AudioClip _mp7SFX;
    [SerializeField] private AudioClip _m700SFX;
    [SerializeField] private AudioClip _reloadSFX;
    
    public void PlayGunSFX(uint gunId)
    {
        if(gunId == GunId.GlockId)
        {
            _sfxSource.PlayOneShot(_glockSFX);
        }
        else if(gunId == GunId.Mp7Id)
        {
            _sfxSource.PlayOneShot(_mp7SFX);
        }
        else if (gunId == GunId.M700Id)
        {
            _sfxSource.PlayOneShot(_m700SFX);
        }
    }

    public void PlayReloadSFX()
    {
        _sfxSource.PlayOneShot(_reloadSFX);
    }
}
