using System.Collections;
using UnityEngine;

public class TimeDecreasingStat : MonoBehaviour
{
    [SerializeField] protected float _max;
    [SerializeField] private float _originReduceAmountPerTick;
    [SerializeField] private float _reduceDelay;

    private PlayerMove _playerMove;
    private PlayerInteract _playerInteract;
    protected float _current;
    private WaitForSeconds _waitForReduceDelay;
    protected bool _isZeroStat;
    private float _currentReduceAmountPerTick;
    protected UIManager _uiManager;

    private void Awake()
    {
        _waitForReduceDelay = new WaitForSeconds(_reduceDelay);
        _current = _max;
        _currentReduceAmountPerTick = _originReduceAmountPerTick;
    }

    protected virtual void Start()
    {
        _uiManager = UIManager.Instance;
        
        StartCoroutine(ReducePerTickRoutine());

        _playerMove = GetComponent<PlayerMove>();
        _playerMove.OnRun += DoubleReduceAmount;
        _playerMove.OnRunCancel += RestoreReduceAmount;

        _playerInteract = GetComponent<PlayerInteract>();
        _playerInteract.OnEnableInteractEvent += RestoreReduceAmount;
    }

    private void OnDisable()
    {
        _playerMove.OnRun -= DoubleReduceAmount;
        _playerMove.OnRunCancel -= RestoreReduceAmount;
        _playerInteract.OnEnableInteractEvent -= RestoreReduceAmount;
    }

    protected virtual void RefreshUI() { }

    protected virtual void OnEnterZeroStat()
    {
        _isZeroStat = true;
    }

    protected virtual void OnExitZeroStat() 
    {
        StartCoroutine(ReducePerTickRoutine());
    }

    public void Heal(float amount)
    {
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

    private IEnumerator ReducePerTickRoutine()
    {
        while (_current > 0)
        {
            _current -= _currentReduceAmountPerTick;

            RefreshUI();

            yield return _waitForReduceDelay;
        }

        if (_current <= 0)
        {
            _current = 0;
            OnEnterZeroStat();
        }

        yield return null;
    }

}
