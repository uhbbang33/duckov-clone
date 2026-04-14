using UnityEngine;

public class FieldManager : SingletonMonoBehaviour<FieldManager>
{
    [SerializeField] private GameObject[] _boxItemSlots;

    [SerializeField] private GameObject _dropItemPrefab;
    [SerializeField] private GameObject _playerObject;

    private Box _currentBox;
    private Box _currentOpenBox;

    public readonly int BoxSlotNum = 5;

    public GameObject[] BoxItemSlots { get { return _boxItemSlots; } }
   
    public Box CurrentBox
    {
        get { return _currentBox; }
        set { _currentBox = value; }
    }

    public Box CurrentOpenBox
    {
        get { return _currentOpenBox; }
        set { _currentOpenBox = value; }
    }
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.PlayerObject = _playerObject;
    }

    // TODO : PoolManager?
    public bool CreateDropItemObject(Item item, int quantity)
    {
        GameObject dropItem = Instantiate(_dropItemPrefab);

        if (!dropItem.GetComponent<DroppedItem>().InitializeDroppedItem(item, quantity))
        {
            Destroy(dropItem);
            return false;
        }

        return true;
    }

}
