using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject[] _slotObject;

    private InputActions _inputActions;
    private bool _inventoryToggle;
    private PlayerMove _playerMove;
    private PlayerInteract _playerInteract;

    private ItemSlot[] _inventorySlots;
    private int _slotCnt;

    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerInteract = GetComponent<PlayerInteract>();
        _inventoryUI.SetActive(false);
        _inventoryToggle = false;

        _slotCnt = _slotObject.Length;

        for (int i = 0; i < _slotCnt; ++i)
            UIManager.Instance.ChangeImageAlpha(_slotObject[i].GetComponent<Image>(), false);

        _inventorySlots = new ItemSlot[_slotCnt];
        for (int i = 0; i < _slotCnt; ++i)
        {
            _inventorySlots[i] = new ItemSlot();
            _inventorySlots[i].UI = _slotObject[i].GetComponent<ItemSlotUI>();
        }
    }

    private void Start()
    {
        _inputActions = GetComponent<Player>().Actions;
        _inputActions.Player.Inventory.performed += OnInventory;
        _inputActions.Player.Cancel.performed += OnInventory;
        // TODO : Inventory에 player가 가지고 있는 물품 넣기

        _playerInteract.OnInteractEvent += OnInventoryCloseBlocked;
    }

    private void OnDisable()
    {
        _inputActions.Player.Inventory.performed -= OnInventory;
        _inputActions.Player.Cancel.performed -= OnInventory;

        _playerInteract.OnInteractEvent -= OnInventoryCloseBlocked;
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        _inventoryToggle = !_inventoryToggle;
        _inventoryUI.SetActive(_inventoryToggle);

        // TODO : UIManager에서 
        // TODO : Player HP, SP Bar hide
        if (_inventoryToggle)
        {
            _playerMove.StopMove();
        }
        else
        {
            _playerMove.RestartMove();
        }
    }

    private void OnInventoryCloseBlocked(bool isBlock)
    {
        if (isBlock)
            _inputActions.Player.Inventory.performed -= OnInventory;
        else
            _inputActions.Player.Inventory.performed += OnInventory;
    }

}
