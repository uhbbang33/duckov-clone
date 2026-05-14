using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private int _inventorySlotCount = 25;
    [SerializeField] private float _inventoryMaxWeight = 25f;

    private Inventory _inventory;
    private string _currentSceneName;
    private GameObject _playerObject;
    private InputActions _inputActions;
    private DayNightCycle _dayNightCycle;

    public Inventory Inventory => _inventory;
    public string CurrentSceneName
    {
        get { return _currentSceneName; }
        set { _currentSceneName = value; }
    }
    public GameObject PlayerObject
    {
        get { return _playerObject; }
        set { _playerObject = value; }
    }
    public DayNightCycle DayNightCycle
    {
        get { return _dayNightCycle; }
        set { _dayNightCycle = value; }
    }

    public InputActions Actions => _inputActions;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        _currentSceneName = SceneName.TitleScene;
        _inventory = new Inventory(_inventorySlotCount, _inventoryMaxWeight);
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        EnableInputActions();
    }

    private void OnDisable()
    {
        DisableInputActions();
    }

    public void EnableInputActions()
    {
        _inputActions?.Enable();
    }

    public void DisableInputActions()
    {
        _inputActions?.Disable();
    }

    // TODO : GameManager?
    public bool CreateDropItemObject(Item item, int quantity)
    {
        GameObject dropItem = PoolManager.Instance.GetObject(PoolId.DroppedItem);

        if (!dropItem.GetComponent<DroppedItem>().InitializeDroppedItem(item, quantity))
        {
            Destroy(dropItem);
            return false;
        }

        return true;
    }

    public void QuitGame()
    {
        DataManager.Instance.SaveDataByScene();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
