using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    // TODO
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private GameObject[] _boxItemSlots;

    [SerializeField] private GameObject _dropItemPrefab;

    private Box _currentBox;

    public readonly int BoxSlotNum = 5;

    public GameObject PlayerObject { get { return _playerObject; } }

    public GameObject[] BoxItemSlots
    {
        get { return _boxItemSlots; }
    }

    public Inventory Inventory { get { return _playerObject.GetComponent<Inventory>(); } }

    public Box CurrentBox
    {
        get { return _currentBox; }
        set { _currentBox = value; }
    }

    // TODO : PoolManager?
    public void CreateDropItemObject(Item item, int quantity)
    {
        GameObject dropItem = Instantiate(_dropItemPrefab);

        dropItem.GetComponent<DroppedItem>().InitializeDroppedItem(item, quantity);
    }
}
