using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [Space(10)]
    [Header("Audio Clip")]
    [SerializeField] private AudioClip _glockSFX;
    [SerializeField] private AudioClip _mp7SFX;
    [SerializeField] private AudioClip _m700SFX;
    [SerializeField] private AudioClip _reloadStartSFX;
    [SerializeField] private AudioClip _reloadEndSFX;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    public void PlayGunSFX(uint gunId, AudioSource source)
    {
        if (gunId == GunId.GlockId)
        {
            source.PlayOneShot(_glockSFX);
        }
        else if (gunId == GunId.Mp7Id)
        {
            source.PlayOneShot(_mp7SFX);
        }
        else if (gunId == GunId.M700Id)
        {
            source.PlayOneShot(_m700SFX);
        }
    }

    public void PlayReloadSFX(bool isStart, AudioSource source)
    {
        if (isStart)
            source.PlayOneShot(_reloadStartSFX);
        else
            source.PlayOneShot(_reloadEndSFX);
    }
}
