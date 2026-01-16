using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    // TODO
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private GameObject[] _boxItemSlots;

    private Box _currentBox;

    public readonly int BoxSlotNum = 5;

    public GameObject PlayerObject { get { return _playerObject; } }

    public GameObject[] BoxItemSlots
    {
        get { return _boxItemSlots; }
    }

    public Inventory Inventory { get { return _playerObject.GetComponent<Inventory>(); } }

    public Box CurrentBox { 
        get { return _currentBox; }
        set {  _currentBox = value; }
    }

}
