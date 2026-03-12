using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private InputActions _inputActions;
    public InputActions Actions { get { return _inputActions; } }

    private PlayerHealthPoint _hp;
    private StaminaPoint _sp;
    private Hunger _hunger;
    private Hydration _hydration;

    private PlayerBaseData _playerBaseData;
    private PlayerMoveData _playerMoveData;
    private PlayerSoundData _playerSoundData;

    public PlayerBaseData BaseData {  get { return _playerBaseData; } }
    public PlayerMoveData MoveData {  get { return _playerMoveData; } }
    public PlayerSoundData SoundData { get { return _playerSoundData; } }

    public event Action OnPlayerDataInitialized;

    private void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Player.Enable();

        _hp = GetComponent<PlayerHealthPoint>();
        _sp = GetComponent<StaminaPoint>();
        _hunger = GetComponent<Hunger>();
        _hydration = GetComponent<Hydration>();
    }

    private void Start()
    {
        DataManager dataManager = DataManager.Instance;

        _playerBaseData = dataManager.PlayerBaseList.PlayerBaseDatas[0];
        _playerMoveData = dataManager.PlayerMoveList.PlayerMoveDatas[0];
        _playerSoundData = dataManager.PlayerSoundList.PlayerSoundDatas[0];

        OnPlayerDataInitialized?.Invoke();
    }

    private void OnDestroy()
    {
        _inputActions.Player.Disable();
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
