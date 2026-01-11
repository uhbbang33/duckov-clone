using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    // TODO
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private GameObject[] _boxItemSlots;

    [SerializeField] private Transform _boxUIRoot;
    [SerializeField] private Transform _inventoryUIRoot;

    public readonly int BoxSlotNum = 5;

    public GameObject PlayerObject { get { return _playerObject; } }

    public GameObject[] BoxItemSlots
    {
        get { return _boxItemSlots; }
    }

    public Transform BoxRoot {  get { return _boxUIRoot; } }
    public Transform InventoryRoot { get { return _inventoryUIRoot; } }

    public Inventory Inventory { get { return _playerObject.GetComponent<Inventory>(); } }

}
