using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject[] _slotObject;

    // TODO : Temp
    [SerializeField] private float _maxWeight;

    private InputActions _inputActions;
    private PlayerMove _playerMove;
    private PlayerInteract _playerInteract;

    private ItemSlot[] _inventorySlots;
    private int _slotCnt;
    private bool _inventoryToggle;
    private float _carryWeight;

    // key - id, value - slot count
    private Dictionary<uint, int> _inventoryDict;

    public event Action<float, float> OnWeightChange;

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
            _inventorySlots[i].Type = SlotType.INVENTORY;
        }
    }

    private void Start()
    {
        _inputActions = GetComponent<Player>().Actions;
        _inputActions.Player.Inventory.performed += OnInventory;
        _inputActions.Player.Cancel.performed += OnInventoryClose;
        // TODO : Inventory에 player가 가지고 있는 물품 넣기

        _playerInteract.OnInteractEvent += OnInventoryCloseBlocked;

        
    }

    private void OnDisable()
    {
        _inputActions.Player.Inventory.performed -= OnInventory;
        _inputActions.Player.Cancel.performed -= OnInventoryClose;

        _playerInteract.OnInteractEvent -= OnInventoryCloseBlocked;
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        OpenInventory();
    }

    public void OnInventoryOpenWithBox()
    {
        if (_playerInteract.UI == null || _inventoryToggle)
            return;

        OpenInventory();
    }

    private void OnInventoryClose(InputAction.CallbackContext context)
    {
        if (!_inventoryToggle)
            return;

        _inventoryUI.SetActive(false);

        _playerMove.RestartMove();

        _inventoryToggle = false;
    }

    private void OpenInventory()
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

        OnWeightChange?.Invoke(_carryWeight, _maxWeight);
    }

    private void OnInventoryCloseBlocked(bool isBlock)
    {
        if (isBlock)
            _inputActions.Player.Inventory.performed -= OnInventory;
        else
            _inputActions.Player.Inventory.performed += OnInventory;
    }

    public bool TryAddItem(Item item, int amount)
    {
        if (!CanAddItemByWeight(item.Weight * amount))
            return false;

        if (_inventoryDict.ContainsKey(item.ID) && item.Type != ItemType.Gun)
        {
            for (int i = 0; i < _slotCnt; ++i)
            {
                if (_inventorySlots[i].CurrentItem == null)
                    continue;

                if (_inventorySlots[i].CurrentItem.ID == item.ID)
                {
                    _inventorySlots[i].AddItem(item, amount);
                    return true;
                }
            }
        }

        int slotIndex = FindFirstEmptySlot();
        if (slotIndex == -1)
            return false;

        _inventorySlots[slotIndex].AddItem(item, amount);

        if (item.Type == ItemType.Gun && _inventoryDict.ContainsKey(item.ID))
            _inventoryDict[item.ID] += 1;
        else
            _inventoryDict.Add(item.ID, 1);


        return true;
    }

    public void AddItemToEmptySlot(Item item, int amount)
    {
        int slotIndex = FindFirstEmptySlot();

        if (slotIndex == -1 || !CanAddItemByWeight(item.Weight * amount))
            return;

        _inventorySlots[slotIndex].AddItem(item, amount);
        AddToDictionaryByID(item.ID);
    }

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < _slotCnt; ++i)
        {
            if (_inventorySlots[i].CurrentItem == null)
            {
                return i;
            }
        }

        return -1;
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

    private bool CanAddItemByWeight(float weight)
    {
        if(_maxWeight < _carryWeight + weight)
        {
            return false;
        }

        return true;
    }

    public void AddWeight(float weight)
    {
        _carryWeight += weight;

        OnWeightChange?.Invoke(_carryWeight, _maxWeight);

        // TODO 이동속도 변화 함수
    }

    public void LoseWeight(float weight)
    {
        _carryWeight -= weight;

        OnWeightChange?.Invoke(_carryWeight, _maxWeight);
    }

}
