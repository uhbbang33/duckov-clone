using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject[] _slotObject;

    private InputActions _inputActions;
    private PlayerMove _playerMove;
    private PlayerInteract _playerInteract;

    private ItemSlot[] _inventorySlots;
    private int _slotCnt;
    private bool _inventoryToggle;

    // key - id, value - slot count
    private Dictionary<uint, int> _inventoryDict;

    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerInteract = GetComponent<PlayerInteract>();
        _inventoryUI.SetActive(false);
        _inventoryDict = new Dictionary<uint, int>();

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
        _inputActions.Player.Interact.performed += OnInventoryOpenWithBox;
        _inputActions.Player.Cancel.performed += OnInventoryClose;
        // TODO : Inventory에 player가 가지고 있는 물품 넣기

        _playerInteract.OnInteractEvent += OnInventoryCloseBlocked;
    }

    private void OnDisable()
    {
        _inputActions.Player.Inventory.performed -= OnInventory;
        _inputActions.Player.Interact.performed -= OnInventoryOpenWithBox;
        _inputActions.Player.Cancel.performed -= OnInventoryClose;

        _playerInteract.OnInteractEvent -= OnInventoryCloseBlocked;
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        // TODO : 상호작용 UI도 없어져야함
        // TODO : UIManager에서 
        // TODO : Player HP, SP Bar hide
        _inventoryToggle = !_inventoryToggle;

        _inventoryUI.SetActive(_inventoryToggle);

        if (_inventoryToggle)
            _playerMove.StopMove();
        else
            _playerMove.RestartMove();
    }

    private void OnInventoryOpenWithBox(InputAction.CallbackContext context)
    {
        if (_playerInteract.UI == null || _inventoryToggle)
            return;

        OnInventory(context);
    }

    private void OnInventoryClose(InputAction.CallbackContext context)
    {
        if (!_inventoryToggle)
            return;

        _inventoryUI.SetActive(false);

        _playerMove.RestartMove();

        _inventoryToggle = false;
    }

    private void OnInventoryCloseBlocked(bool isBlock)
    {
        if (isBlock)
            _inputActions.Player.Inventory.performed -= OnInventory;
        else
            _inputActions.Player.Inventory.performed += OnInventory;
    }

    public bool TryAddItemByDoubleClick(Item item, int amount)
    {
        if (_inventoryDict.ContainsKey(item.ID) && item.Type != ItemType.Gun)
        {
            for (int i = 0; i < _slotCnt; ++i)
            {
                if (_inventorySlots[i].CurrentItem == null)
                    continue;

                if (_inventorySlots[i].CurrentItem.ID == item.ID)
                {
                    _inventorySlots[i].AddItem(item, amount);
                    _inventorySlots[i].UI.RefreshUI();
                    return true;
                }
            }
        }

        for (int i = 0; i < _slotCnt; ++i)
        {
            if (_inventorySlots[i].CurrentItem == null)
            {
                _inventorySlots[i].AddItem(item, amount);
                _inventorySlots[i].UI.RefreshUI();

                if (item.Type == ItemType.Gun && _inventoryDict.ContainsKey(item.ID))
                    _inventoryDict[item.ID] += 1;
                else
                    _inventoryDict.Add(item.ID, 1);

                return true;
            }
        }
        return false;
    }

    public void AddItemToEmptySlot(Item item, int amount)
    {
        for (int i = 0; i < _slotCnt; ++i)
        {
            if (_inventorySlots[i].CurrentItem == null)
            {
                _inventorySlots[i].AddItem(item, amount);
                _inventorySlots[i].UI.RefreshUI();
                AddToDictionaryByID(item.ID);

                return;
            }
        }
    }

    public void AddToDictionaryByID(uint id)
    {
        if (_inventoryDict.ContainsKey(id))
            _inventoryDict[id] += 1;
        else
            _inventoryDict.Add(id, 1);
    }

    public void RemoveItemSlot(uint id)
    {
        _inventoryDict[id] -= 1;

        if (_inventoryDict[id] == 0)
            _inventoryDict.Remove(id);
    }

}
