using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;

    private InputActions _inputActions;
    private bool _inventoryToggle;
    private PlayerMove _playerMove;

    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _inventoryUI.SetActive(false);
        _inventoryToggle = false;
    }

    private void Start()
    {
        _inputActions = GetComponent<Player>().Actions;
        _inputActions.Player.Inventory.performed += OnInventory;
        // TODO : Inventory에 가지고 있는 물품 넣기
    }

    private void OnDisable()
    {
        _inputActions.Player.Inventory.performed -= OnInventory;
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
}
