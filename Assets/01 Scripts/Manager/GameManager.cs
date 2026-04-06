using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    // TODO
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private GameObject[] _boxItemSlots;
    [SerializeField] private GameObject[] _storageItemSlots;
    [SerializeField] private GameObject[] _shopItemSlots;

    [SerializeField] private Storage _storage;

    [SerializeField] private GameObject _dropItemPrefab;

    private SaveManager _saveManager;
    private InputActions _inputActions;
    private Box _currentBox;
    private Box _currentOpenBox;
    private Inventory _inventory;

    private bool _isStorageOpened;
    private bool _isShopOpened;

    public readonly int BoxSlotNum = 5;

    public GameObject PlayerObject { get { return _playerObject; } }
    public GameObject[] BoxItemSlots { get { return _boxItemSlots; } }
    public GameObject[] StorageItemSlots { get { return _storageItemSlots; } }
    public GameObject[] ShopItemSlots { get { return _shopItemSlots; } }
    public Inventory Inventory { get { return _inventory; } }
    public InputActions Actions { get { return _inputActions; } }

    public Box CurrentBox
    {
        get { return _currentBox; }
        set { _currentBox = value; }
    }

    public Box CurrentOpenBox
    {
        get { return _currentOpenBox; }
        set { _currentOpenBox = value; }
    }

    public Storage storage { get { return _storage; } }

    public bool IsStorageOpened
    {
        get { return _isStorageOpened; }
        set { _isStorageOpened = value; }
    }

    public bool IsShopOpened
    {
        get { return _isShopOpened; }
        set { _isShopOpened = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        _inventory = _playerObject.GetComponent<Inventory>();
        _inputActions = new InputActions();
        _inputActions.Player.Enable();

    }

    private void Start()
    {
        _saveManager = SaveManager.Instance;
    }

    private void OnDestroy()
    {
        _inputActions.Player.Disable();
    }

    // TODO : PoolManager?
    public bool CreateDropItemObject(Item item, int quantity)
    {
        GameObject dropItem = Instantiate(_dropItemPrefab);

        if (!dropItem.GetComponent<DroppedItem>().InitializeDroppedItem(item, quantity))
        {
            Destroy(dropItem);
            return false;
        }

        return true;
    }

    public void QuitGame()
    {
        SaveAllData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // TODO : 필드 씬으로 전환 시 호출
    public void SaveAllData()
    {
        SavePlayerData();
        _saveManager.SaveStorage();
    }

    // TODO : 벙커 씬으로 전환 시 호출
    public void SavePlayerData()
    {
        _saveManager.SavePlayerStats();
        _saveManager.SavePlayerInventory();
    }
}
