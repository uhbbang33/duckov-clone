using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerHealthPoint _hp;
    private StaminaPoint _sp;
    private Hunger _hunger;
    private Hydration _hydration;
    private PlayerMove _playerMove;
    private PlayerShooting _playerShooting;
    private Animator _anim;

    private DataManager _dataManager;
    private PlayerBaseData _playerBaseData;
    private PlayerMoveData _playerMoveData;
    private PlayerSoundData _playerSoundData;

    private PlayerState _state;

    public PlayerStatsSaveData StatsSaveData
    {
        get
        {
            return new PlayerStatsSaveData
            {
                //CurrentHP = _hp.CurrentHP,
                //CurrentSP = Mathf.Floor(_sp.CurrentSP),
                CurrentHydration = Mathf.Floor(_hydration.Current),
                CurrentHunger = Mathf.Floor(_hunger.Current)
            };
        }
        set
        {
            //_hp.LoadCurrentHPData(value.CurrentHP);
            //_sp.LoadCurrentSPData(value.CurrentSP);
            _hydration.LoadCurrent(value.CurrentHydration);
            _hunger.LoadCurrent(value.CurrentHunger);
        }
    }

    public PlayerBaseData BaseData {  get { return _playerBaseData; } }
    public PlayerMoveData MoveData {  get { return _playerMoveData; } }
    public PlayerSoundData SoundData { get { return _playerSoundData; } }

    public PlayerState State {  get { return _state; } }

    public event Action OnPlayerDataInitialized;

    private void Awake()
    {
        _hp = GetComponent<PlayerHealthPoint>();
        _sp = GetComponent<StaminaPoint>();
        _hunger = GetComponent<Hunger>();
        _hydration = GetComponent<Hydration>();
        _playerMove = GetComponent<PlayerMove>();
        _playerShooting = GetComponent<PlayerShooting>();
        _anim = GetComponent<Animator>();

        _state = PlayerState.Idle;
    }

    private void Start()
    {
        _dataManager = DataManager.Instance;

        _playerBaseData = _dataManager.PlayerBaseList.PlayerBaseStatsDatas[0];
        _playerMoveData = _dataManager.PlayerMoveList.PlayerMoveStatsDatas[0];
        _playerSoundData = _dataManager.PlayerSoundList.PlayerSoundDatas[0];

        OnPlayerDataInitialized?.Invoke();
    }

    public void ChangePlayerState(PlayerState newState)
    {
        if(_state == newState)
            return;

        _state = newState;

        if(newState == PlayerState.Die)
        {
            Die();
        }
    }

    public bool UseItem(UsableItem item)
    {
        if (!_hp.Heal(item.HealHP)
            && item.Hunger == 0
            && item.Hydration == 0)
        {
            return false;
        }

        _hunger.Heal(item.Hunger);
        _hydration.Heal(item.Hydration);

        return true;
    }

    private void Die()
    { 
        _anim.SetTrigger(PlayerAnimParm.Die);

        _playerMove.enabled = false;
        _playerShooting.enabled = false;

        SoundManager.Instance.PlaySFXOneShot(SFXName.GameOver);

        StartCoroutine(GameOverRoutine());
    }


    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(2f);

        SaveAndLoadManager saveAndLoadManager = SaveAndLoadManager.Instance;
        saveAndLoadManager.DeletePlayerStats();
        saveAndLoadManager.DeletePlayerInventory();

        if (GameManager.Instance.CurrentSceneName == SceneName.FieldScene)
        {
            UIManager.Instance.ShowGameOverUI();
        }
    }
}
