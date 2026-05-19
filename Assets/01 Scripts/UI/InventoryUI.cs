using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, IUICloseable
{
    [SerializeField] private GameObject _inventoryUIObject;
    [SerializeField] private GameObject[] _slotObjects;
    [SerializeField] private Button _sortButton;

    private Inventory _inventory;
    private InventoryController _inventoryController;
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = UIManager.Instance;
        _inventory = GameManager.Instance.Inventory;

        _inventoryController = GameManager.Instance.PlayerObject.GetComponent<InventoryController>();
        _inventoryController.OnInventoryToggle += OnInventoryToggled;

        ItemSlotUI[] slotUIs = new ItemSlotUI[_slotObjects.Length];
        for (int i = 0; i < _slotObjects.Length; ++i)
        {
            slotUIs[i] = _slotObjects[i].GetComponentInChildren<ItemSlotUI>();
        }
        _inventory.LinkSlotUI(slotUIs);

        _inventory.OnMoneyChange += _uiManager.ChangeInventoryMoneyText;
        _inventory.OnItemCountChange += _uiManager.ChangeInventoryItemCountText;
        _inventory.OnWeightChange += _uiManager.ChangeWeightText;

        _sortButton.onClick.AddListener(() => SortUtility.Sort(_inventory));
        _inventoryUIObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _inventory.OnMoneyChange -= _uiManager.ChangeInventoryMoneyText;
        _inventory.OnItemCountChange -= _uiManager.ChangeInventoryItemCountText;
        _inventory.OnWeightChange -= _uiManager.ChangeWeightText;
        _inventoryController.OnInventoryToggle -= OnInventoryToggled;
    }

    private void OnInventoryToggled(bool isInventoryOpen)
    {
        if (isInventoryOpen)
        {
            if (!_uiManager.TryPushStack(this))
                return;
        }
        else if (_uiManager.PeekStack() == (IUICloseable)this)
            _uiManager.PopStack();

        _inventoryUIObject.SetActive(isInventoryOpen);
        _uiManager.DefaultUHDShowToggle(!isInventoryOpen);
        _uiManager.ShowCursor(isInventoryOpen);
        _uiManager.PlayerCanvasShowToggle(!isInventoryOpen);
    }

    public void CloseUI()
    {
        _inventoryController.CloseInventory();
    }
}
