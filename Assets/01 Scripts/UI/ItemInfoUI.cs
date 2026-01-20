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
    [SerializeField] private Vector2 _positionOffest;

    private PlayerInteract _playerInteract;

    private void Start()
    {
     // TODO
        _playerInteract = GameManager.Instance.PlayerObject.GetComponent<PlayerInteract>();
        _playerInteract.OnCloseUIEvent += HideUI;
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

    public void SetInfoUI(Item item)
    {
        InitializeUI();

        if (item == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _nameText.text = item.Name;
        _idText.text = "#" + item.ID.ToString();
        _weightText.text = item.Weight.ToString();
        _valueText.text = "$" + item.Value.ToString();

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
            _medicineFoodEffectText.text += "회복량: " + item.HealHP.ToString();
        if(item.DurabilityCost != 0)
            _medicineFoodEffectText.text += "회복량: " + item.DurabilityCost.ToString();
        if (item.Hunger != 0)
            _medicineFoodEffectText.text += "에너지: " + item.Hunger.ToString();
        if (item.Hydration != 0)
            _medicineFoodEffectText.text += "수분: " + item.Hydration.ToString();
    }

    private void SetGunValueText(GunItem item)
    {
        _gunValueText.text =
            item.GunBulletType + "\n"
            + item.Damage.ToString() + "\n"
             + item.Rps.ToString() + "\n"
              + item.MagazineCapacity.ToString() + "\n"
               + item.Range.ToString() + "\n"
                + item.ReloadTime.ToString() + "\n"
                 + item.AdsTime.ToString();
    }

    private void SetAmmoValueText(AmmoItem item)
    {
        _ammoValueText.text = item.AmmoBulletType;
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
        FollowMouse();
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }

    private void FollowMouse()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        transform.position = mousePos + _positionOffest;
    }

}
