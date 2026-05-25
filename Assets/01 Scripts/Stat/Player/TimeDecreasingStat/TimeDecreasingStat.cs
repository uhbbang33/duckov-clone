using System.Collections;
using UnityEngine;

public class TimeDecreasingStat : MonoBehaviour
{
    [SerializeField] private float _reduceDelay;

    private Player _player;
    protected PlayerBaseData _playerBaseData;
    private PlayerMove _playerMove;
    private PlayerInteract _playerInteract;
    protected UIManager _uiManager;
    private GameManager _gameManager;

    protected float _max;
    protected float _current;
    protected bool _isZeroStat;
    protected float _currentReduceAmountPerTick;
    protected float _originReduceAmountPerTick;

    private WaitForSeconds _waitForReduceDelay;

    private const float _currentInit = 100f;

    public float Current { get { return _current; } }

    private void Awake()
    {
        _current = _currentInit;

        _player = GetComponent<Player>();
        _player.OnPlayerDataInitialized += PlayerSetup;

        _waitForReduceDelay = new WaitForSeconds(_reduceDelay);
    }

    private void OnDisable()
    {
        _playerMove.OnRun -= DoubleReduceAmount;
        _playerMove.OnRunCancel -= RestoreReduceAmount;
        _playerInteract.OnEnableInteractEvent -= RestoreReduceAmount;
    }

    protected virtual void PlayerSetup()
    {
        _playerBaseData = _player.BaseData;

        _uiManager = UIManager.Instance;
        _gameManager = GameManager.Instance;

        if (_gameManager.CurrentSceneName != SceneName.BunkerScene)
            StartCoroutine(ReducePerTickRoutine());

        _playerMove = GetComponent<PlayerMove>();
        _playerMove.OnRun += DoubleReduceAmount;
        _playerMove.OnRunCancel += RestoreReduceAmount;

        _playerInteract = GetComponent<PlayerInteract>();
        _playerInteract.OnEnableInteractEvent += RestoreReduceAmount;

        RefreshUI();
    }

    protected virtual void RefreshUI() { }

    protected virtual void OnEnterZeroStat()
    {
        _isZeroStat = true;
    }

    protected virtual void OnExitZeroStat() 
    {
        if (_gameManager.CurrentSceneName != SceneName.BunkerScene)
            StartCoroutine(ReducePerTickRoutine());
    }

    public void Heal(float amount)
    {
        if (amount == 0)
            return;

        _current += amount;

        if (_isZeroStat)
        {
            OnExitZeroStat();
            _isZeroStat = false;
        }

        if (_current > _max)
            _current = _max;

        RefreshUI();
    }

    private void DoubleReduceAmount()
    {
        _currentReduceAmountPerTick = 2 * _originReduceAmountPerTick;
    }

    private void RestoreReduceAmount()
    {
        _currentReduceAmountPerTick = _originReduceAmountPerTick;
    }

    public void LoadCurrent(float current)
    {
        _current = current;
        RefreshUI();
    }

    private IEnumerator ReducePerTickRoutine()
    {
        while (_current > 0)
        {
            yield return _waitForReduceDelay;

            _current -= _currentReduceAmountPerTick;
            
            RefreshUI();
        }

        if (_current <= 0)
        {
            _current = 0;
            OnEnterZeroStat();
        }

        yield return null;
    }

}
