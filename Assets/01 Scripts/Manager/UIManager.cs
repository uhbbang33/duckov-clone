using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [Space(10)]
    [Header("Offset")]
    [SerializeField] private Vector3 _inventorySlotMenuOffset;
    [SerializeField] private Vector3 _boxSlotMenuOffset;

    [Space(10)]
    [Header("Button")]
    [SerializeField] private GameObject _equipButton;
    [SerializeField] private GameObject _unloadButton;
    [SerializeField] private GameObject _useButton;
    [SerializeField] private GameObject _splitButton;
    [SerializeField] private GameObject _discardButton;

    [Space(10)]
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _boxItemCountText;
    [SerializeField] private TextMeshProUGUI _inventoryItemCountText;
    [SerializeField] private TextMeshProUGUI _weightText;
    [SerializeField] private TextMeshProUGUI _inventoryMoneyText;
    [SerializeField] private TextMeshProUGUI _storageItemCountText;
    [SerializeField] private TextMeshProUGUI _mainUIHPBarText;

    [Space(10)]
    [Header("Slider")]
    [SerializeField] private Slider _mainUIHPBarSlider;
    [SerializeField] private Slider _weightSlider;
    [SerializeField] private Slider _mainUIHydrationSlider;
    [SerializeField] private Slider _mainUIHungerSlider;

    [Space(5)]
    [Header("SliderBackgroundImage")]
    [SerializeField] private Image _mainUIHydrationSliderBackground;
    [SerializeField] private Image _mainUIHungerSliderBackground;


    [Space(10)]
    [Header("Sprite")]
    [SerializeField] private Sprite _pistolIcon;


    [Space(20)]
    [Header("GameObject")]
    [SerializeField] private GameObject _slotMenuUI;
    [SerializeField] private GameObject _buttonsObject;
    [SerializeField] private GameObject _defaultHUD;
    [SerializeField] private GameObject _interactableBoxUI;
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private GameObject _playerCanvas;
    [SerializeField] private GameObject _boxUI;
    [SerializeField] private GameObject _storageUI;
    [SerializeField] private GameObject _shopUI;

    [Space(30)]
    [SerializeField] private DefaultHUDSlotUI[] _defaultHUDSlotUI;
    [SerializeField] private EquipSlotUI _leftEquipSlotUI;
    [SerializeField] private ItemSplitUI _splitUI;
    [SerializeField] private GameOverUI _gameOverUI;
    [SerializeField] private Transform _dragCanvasTransform;

    private SoundManager _soundManager;
    private ItemSlot _currentSlot;
    private ItemInfoUI _currentInfoUI = null;
    private InputActions _inputActions;
    private Color _hydrationBackgroundOriginColor;
    private Color _hungerBackgroundOriginColor;

    public Transform DragCanvasTransform => _dragCanvasTransform;
    public Sprite PistolIcon => _pistolIcon;
    public ItemInfoUI CurrentInfoUI
    {
        get { return _currentInfoUI; }
        set { _currentInfoUI = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        _currentSlot = new ItemSlot();
        _inputActions = new InputActions();

        _hungerBackgroundOriginColor = _mainUIHungerSliderBackground.color;
        _hydrationBackgroundOriginColor = _mainUIHydrationSliderBackground.color;

        // 커서가 화면 밖으로 못 나가도록
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnEnable()
    {
        _inputActions.UI.Enable();
        _inputActions.UI.CloseSlotMenuUI.performed += OnClick;
    }

    private void Start()
    {
        _soundManager = SoundManager.Instance;
    }

    private void OnDisable()
    {
        _inputActions.UI.CloseSlotMenuUI.performed -= OnClick;
        _inputActions.UI.Disable();
    }

    public void ShowCursor(bool showCursor)
    {
        Cursor.visible = showCursor;
        _crosshair.SetActive(!showCursor);
    }

    public void ChangeImageAlpha(Image image, bool showImage)
    {
        Color color = image.color;
        color.a = showImage ? 255f : 0f;
        image.color = color;
    }

    public void OpenSlotMenu(ItemSlot slot, Vector3 pos)
    {
        _buttonsObject.transform.position = pos;

        if (slot.Type == SlotType.INVENTORY || slot.Type == SlotType.EQUIP)
            _buttonsObject.transform.position += _inventorySlotMenuOffset;
        else
            _buttonsObject.transform.position += _boxSlotMenuOffset;

        if (IsUpperHalf(_buttonsObject.transform.position))
        {
            // TODO
        }

        _currentSlot = slot;
        ShowButtonsByItemtype();

        _slotMenuUI.SetActive(true);
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        GameObject clickedUI = GetClickedUI();
        if (clickedUI == null)
        {
            CloseSlotMenu();
            return;
        }

        Button clickedButton = clickedUI.GetComponent<Button>();
        if (clickedButton != null)
            return;

        ItemSlotUI slotUI = clickedUI.GetComponent<ItemSlotUI>();
        if(slotUI == null)
        {
            CloseSlotMenu();
            return;
        }
    }

    public void CloseSlotMenu()
    {
        _slotMenuUI.SetActive(false);
    }

    private GameObject GetClickedUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0) return results[0].gameObject;
        else return null;
    }

    private bool IsUpperHalf(Vector3 pos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        return screenPos.y > Screen.height * 0.5f;
    }

    private void ShowButtonsByItemtype()
    {
        string itemType = _currentSlot.CurrentItem.Type;

        // Temp
        _equipButton.SetActive(false);
        _unloadButton.SetActive(false);
        _useButton.SetActive(false);
        _splitButton.SetActive(false);
        _discardButton.SetActive(false);

        if (itemType == ItemType.Gun)
        {
            if (_currentSlot.Type != SlotType.EQUIP)
            {
                _equipButton.SetActive(true);
            }
            _unloadButton.SetActive(true);
        }
        else if (itemType == ItemType.Medicine || itemType == ItemType.Food)
        {
            _useButton.SetActive(true);
            _splitButton.SetActive(true);
        }
        else
        {
            _splitButton.SetActive(true);
        }

        if (_currentSlot.Type == SlotType.INVENTORY || _currentSlot.Type == SlotType.EQUIP)
        {
            _discardButton.SetActive(true);

            if (GameManager.Instance.Inventory.FindFirstEmptySlot() == -1
                || _currentSlot.Quantity < 2)
                _splitButton.SetActive(false);
        }
        else if (_currentSlot.Type == SlotType.BOX)
        {
            if (FieldManager.Instance.CurrentBox.FindFirstEmptySlot() == -1
                || _currentSlot.Quantity < 2)
                _splitButton.SetActive(false);
        }
    }

    public void ChangeBoxItemCountText(int itemCnt, int maxCnt)
    {
        _boxItemCountText.text = "상자 (" + itemCnt + " / " + maxCnt + ")";
    }

    public void ChangeInventoryItemCountText(int itemCnt, int maxCnt)
    {
        _inventoryItemCountText.text
            = "가방 (" + itemCnt + " / " + maxCnt + ")";
    }

    public void ChangeWeightText(float current, float max)
    {
        _weightText.text = current.ToString() + "/" + max.ToString() + "kg";

        _weightSlider.value = current / max;
    }

    public void ChangeInventoryMoneyText(int money)
    {
        _inventoryMoneyText.text = money.ToString();
    }

    public void ChangeStorageItemCountText(int itemCnt, int maxCnt)
    {
        _storageItemCountText.text = "창고 (" + itemCnt + " / " + maxCnt + ")";
    }

    public void ChangeMainUIHPBar(float currentHp, float maxHp)
    {
        _mainUIHPBarSlider.value = currentHp / maxHp;
        _mainUIHPBarText.text = currentHp.ToString() + " / " + maxHp.ToString();
    }

    public void ChangeMainUIHydrationSlider(float current, float max)
    {
        _mainUIHydrationSlider.value = current / max;
    }

    public void ChangeMainUIHungerSlider(float current, float max)
    {
        _mainUIHungerSlider.value = current / max;
    }

    public void ChangeHungerSliderBackgroundColor(bool isRed)
    {
        Color targetColor = new Color();

        if (isRed)
        {
            targetColor = Color.red;
            targetColor = new Color(targetColor.r, targetColor.g, targetColor.b, _hungerBackgroundOriginColor.a);
        }
        else
        {
            targetColor = _hungerBackgroundOriginColor;
        }

        _mainUIHungerSliderBackground.color = targetColor;
    }

    public void ChangeHydrationSliderBackgroundColor(bool isRed)
    {
        Color targetColor = new Color();

        if (isRed)
        {
            targetColor = Color.red;
            targetColor = new Color(targetColor.r, targetColor.g, targetColor.b, _hydrationBackgroundOriginColor.a);
        }
        else
        {
            targetColor = _hydrationBackgroundOriginColor;
        }

        _mainUIHydrationSliderBackground.color = targetColor;
    }

    public void DefaultUHDShowToggle(bool show)
    {
        _defaultHUD.SetActive(show);

        for(int i = 0; i < _defaultHUDSlotUI.Length; ++i)
        {
            _defaultHUDSlotUI[i].RebuildLayout();
        }
    }

    public void PlayerCanvasShowToggle(bool show)
    {
        _playerCanvas.SetActive(show);
    }

    public void ShowBoxUI(bool show)
    {
        _boxUI.SetActive(show);
    }

    public void ShowStorageUI(bool show)
    {
        _storageUI.SetActive(show);
    }

    public void ShowShopUI(bool show)
    {
        _shopUI.SetActive(show);
    }

    public void ShowGameOverUI()
    {
        _gameOverUI.ShowGameOverUI();
        Cursor.visible = true;
    }

    #region On Button Click
    public void OnEquipButtonClick()
    {
        _leftEquipSlotUI.ChangeLeftGunSlotItem(_currentSlot);
        CloseSlotMenu();
    }

    public void OnSplitButtonClick()
    {
        _splitUI.CurrentSlot = _currentSlot;
        _splitUI.gameObject.SetActive(true);
        CloseSlotMenu();
    }

    public void OnDiscardButtonClick()
    {
        CloseSlotMenu();

        if (FieldManager.Instance == null)
            return;
        
        // TODO
        if (FieldManager.Instance.CreateDropItemObject(_currentSlot.CurrentItem, _currentSlot.Quantity))
        {
            _currentSlot.SubtractItem(_currentSlot.Quantity);
            _soundManager.PlaySFXOneShot(SFXName.PickItem);
        }
        else // TODO: 버릴 수 없습니다 UI
        {
            Debug.Log("버릴 수 없습니다.");
        }

    }

    public void OnUsebuttonClick()
    {
        _currentSlot.UseItem();
        CloseSlotMenu();
    }

    public void OnUnloadButtonClick()
    {
        _currentSlot.UnloadAmmo();
        CloseSlotMenu();
    }

    #endregion On Button Click
}
