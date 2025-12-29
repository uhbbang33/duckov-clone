using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    // TODO
    [SerializeField] private GameObject _playerObject;

    public GameObject PlayerObject { get { return _playerObject; } }

    


}
