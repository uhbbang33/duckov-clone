using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Space(10)]
    [Header("Audio Clip")]
    [SerializeField] private AudioClip _gameOverSFX;
    [SerializeField] private AudioClip _menuHoverSFX;

    [Space(10)]
    [Header("Gun Audio Clip")]
    [SerializeField] private AudioClip _glockSFX;
    [SerializeField] private AudioClip _mp7SFX;
    [SerializeField] private AudioClip _m700SFX;
    [SerializeField] private AudioClip _reloadStartSFX;
    [SerializeField] private AudioClip _reloadEndSFX;
    [SerializeField] private AudioClip _hitSFX;

    [Space(10)]
    [Header("Inventory Audio Clip")]
    [SerializeField] private AudioClip _openInventorySFX;
    [SerializeField] private AudioClip _pickItemSFX;
    [SerializeField] private AudioClip _SellBuySFX;

    [Space(10)]
    [Header("Move Audio Clip")]
    [SerializeField] private AudioClip _footStepSFX;
    [SerializeField] private AudioClip _rollSFX;
    [SerializeField] private AudioClip _quackSFX; // 적 - 플레이어 감지

    [Space(20)]
    [Header("BGM")]
    [SerializeField] private AudioClip _titleBGM;
    [SerializeField] private AudioClip _bunkerBGM;

    [Space(10)]
    [Header("Setting")]
    [SerializeField] private float _defaultVolume;

    private Dictionary<string, AudioClip> _clipDict;
    private AudioSource _defaultAudioSource;
    private float _prevSpeed;

    private const string _pitchShifterParam = "PitchShifter";

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        _defaultAudioSource = GetComponent<AudioSource>();

        _clipDict = new()
        {
            { SFXName.GameOver, _gameOverSFX },
            { SFXName.MenuHover, _menuHoverSFX },
            { SFXName.Hit, _hitSFX },
            { SFXName.OpenInventory, _openInventorySFX },
            { SFXName.PickItem, _pickItemSFX },
            { SFXName.SellBuy, _SellBuySFX },
            { SFXName.FootStep, _footStepSFX },
            { SFXName.Roll, _rollSFX },
            { SFXName.Quack, _quackSFX }
        };
    }
    
    public void PlayTitleBGM(AudioSource source)
    {
        source.clip = _titleBGM;
        source.loop = true;
        source.Play();
    }

    public void PlayBunkerBGM(AudioSource source)
    {
        source.clip = _bunkerBGM;
        source.loop = true;
        source.Play();
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

    public void PlaySFXOneShot(string sfxName)
    {
        _defaultAudioSource.volume = _defaultVolume;
        _defaultAudioSource.PlayOneShot(_clipDict[sfxName]);
    }

    public void PlaySFXOneShot(string sfxName, float volume)
    {
        _defaultAudioSource.volume = volume;
        _defaultAudioSource.PlayOneShot(_clipDict[sfxName]);
    }

    public void PlaySFXOneShot(string sfxName, AudioSource source)
    {
        source.PlayOneShot(_clipDict[sfxName]);
    }

    public void PlayFootStepSFX(bool isRun, AudioSource source)
    {
        float speed = isRun ? 1.6f : 1.0f;
        if (source.isPlaying && _prevSpeed == speed)
            return;
        _prevSpeed = speed;

        source.clip = _clipDict[SFXName.FootStep];
        source.loop = true;
        SetSFXSpeed(source, speed);
        source.Play();
    }

    private void SetSFXSpeed(AudioSource source, float speed)
    {
        source.pitch = speed;

        _audioMixer.SetFloat(_pitchShifterParam, 1f / speed);
    }
}
