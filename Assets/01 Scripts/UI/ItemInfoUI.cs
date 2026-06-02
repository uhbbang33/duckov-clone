using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInfoUI : MonoBehaviour
{
    [Header("UI Object")]
    [SerializeField] private GameObject _medicineFoodUI;
    [SerializeField] private GameObject _gunUI;
    [SerializeField] private GameObject _ammoUI;

    [Space(10)]
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _idText;
    [SerializeField] private TextMeshProUGUI _weightText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private TextMeshProUGUI _medicineFoodEffectText;
    [SerializeField] private TextMeshProUGUI _gunValueText;
    [SerializeField] private TextMeshProUGUI _ammoValueText;

    [Space(10)]
    [Header("Offset")]
    [SerializeField] private Vector2 _positionOffset;

    private RectTransform _rectTransform;
    private UIManager _uiManager;

    private float _rectWidth;
    private float _rectHeight;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _rectWidth = _rectTransform.rect.width;
        _rectHeight = _rectTransform.rect.height;
    }

    private void Start()
    {
        _uiManager = UIManager.Instance;
    }

    private void OnEnable()
    {
        GameManager.Instance.Actions.Player.Cancel.performed += OnCancel;
    }

    private void OnDisable()
    {
        GameManager.Instance.Actions.Player.Cancel.performed -= OnCancel;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        FollowMouse();
    }

    private void InitializeUI()
    {
        _medicineFoodUI.SetActive(false);
        _gunUI.SetActive(false);
        _ammoUI.SetActive(false);
    }

    public void SetInfoUI(Item item, int quantity)
    {
        InitializeUI();

        if (item == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _nameText.text = item.Name;
        _idText.text = "#" + item.ID.ToString();
        _weightText.text = (item.Weight * quantity).ToString() + " kg";
        _valueText.text = "$ " + (item.Value * quantity).ToString();

        if (item.Type == ItemType.Medicine || item.Type == ItemType.Food)
        {
            _medicineFoodUI.SetActive(true);

            UsableItem usableItem = item as UsableItem;
            SetMedicineFoodEffectText(usableItem);
        }
        else if (item.Type == ItemType.Gun)
        {
            _gunUI.SetActive(true);

            GunItem gunItem = item as GunItem;
            SetGunValueText(gunItem);
        }
        else if (item.Type == ItemType.Ammo)
        {
            _ammoUI.SetActive(true);

            AmmoItem ammoItem = item as AmmoItem;
            SetAmmoValueText(ammoItem);
        }
    }

    private void SetMedicineFoodEffectText(UsableItem item)
    {
        _medicineFoodEffectText.text = "- ";

        if (item.HealHP != 0)
            _medicineFoodEffectText.text += "회복량: " + item.HealHP.ToString() + "  ";
        
        if (item.Hunger != 0)
            _medicineFoodEffectText.text += "포만감: " + item.Hunger.ToString() + "  ";
        if (item.Hydration != 0)
            _medicineFoodEffectText.text += "수분: " + item.Hydration.ToString();

        if (item.DurabilityCost != 0 && item.DurabilityCost != 100)
        {
            _medicineFoodEffectText.text += "\n- ";
            _medicineFoodEffectText.text += "내구도 소모: " + item.DurabilityCost.ToString();
        }
    }

    private void SetGunValueText(GunItem item)
    {
        _gunValueText.text =
            item.CurrentAmmoCount + "\n"
            +item.GunAmmoType + "\n"
            + item.Damage.ToString() + "\n"
             + item.Rps.ToString() + "\n"
              + item.MagazineCapacity.ToString() + "\n"
               + item.ReloadTime.ToString() + "\n"
                + item.Range.ToString() + "\n"
                 + item.SoundRange.ToString();
    }

    private void SetAmmoValueText(AmmoItem item)
    {
        _ammoValueText.text = item.AmmoType;
    }

    public void ShowUI()
    {
        if (_uiManager == null) _uiManager = UIManager.Instance;

        if(_uiManager.CurrentInfoUI != null)
        {
            _uiManager.CurrentInfoUI.HideUI();
        }

        _uiManager.CurrentInfoUI = this;

        gameObject.SetActive(true);
        FollowMouse();
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        HideUI();
    }

    private void FollowMouse()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 targetPos = mousePos + _positionOffset;

        float clampedX = Mathf.Clamp(targetPos.x, 0f, Screen.width - _rectWidth);
        float clampedY = Mathf.Clamp(targetPos.y, _rectHeight, Screen.height);

        transform.position = new Vector2(clampedX, clampedY);
    }
}
