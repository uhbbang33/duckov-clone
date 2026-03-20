using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    // TODO
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private GameObject[] _boxItemSlots;

    [SerializeField] private GameObject _dropItemPrefab;

    private InputActions _inputActions;
    private Box _currentBox;
    private Box _currentOpenBox;
    private Inventory _inventory;

    public readonly int BoxSlotNum = 5;

    public GameObject PlayerObject { get { return _playerObject; } }
    public GameObject[] BoxItemSlots { get { return _boxItemSlots; } }
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

    protected override void Awake()
    {
        base.Awake();

        _inventory = _playerObject.GetComponent<Inventory>();
        _inputActions = new InputActions();
        _inputActions.Player.Enable();

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
}
