using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    // TODO
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private GameObject[] _boxItemSlots;

    public readonly int BoxSlotNum = 5;

    public GameObject PlayerObject { get { return _playerObject; } }

    public GameObject[] BoxItemSlots
    {
        get { return _boxItemSlots; }
    }

}
