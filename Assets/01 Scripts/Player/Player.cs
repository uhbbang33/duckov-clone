using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerHealthPoint _hp;
    private StaminaPoint _sp;
    private Hunger _hunger;
    private Hydration _hydration;

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
                CurrentHP = _hp.CurrentHP,
                CurrentSP = Mathf.Floor(_sp.CurrentSP),
                CurrentHydration = Mathf.Floor(_hydration.Current),
                CurrentHunger = Mathf.Floor(_hunger.Current)
            };
        }
        set
        {
            _hp.LoadCurrentHPData(value.CurrentHP);
            _sp.LoadCurrentSPData(value.CurrentSP);
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

        _state = PlayerState.Idle;
    }

    private void Start()
    {
        DataManager dataManager = DataManager.Instance;

        _playerBaseData = dataManager.PlayerBaseList.PlayerBaseStatsDatas[0];
        _playerMoveData = dataManager.PlayerMoveList.PlayerMoveStatsDatas[0];
        _playerSoundData = dataManager.PlayerSoundList.PlayerSoundDatas[0];

        OnPlayerDataInitialized?.Invoke();
    }

    public void ChangePlayerState(PlayerState newState)
    {
        if(_state == newState)
            return;

        //Debug.Log(newState.ToString());

        _state = newState;

        if(newState == PlayerState.Die)
        {
            // GameOver - GameManager
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
}
