using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private int _inventorySlotCount = 25;
    [SerializeField] private float _inventoryMaxWeight = 25f;

    private Inventory _inventory;
    private string _currentSceneName;
    private GameObject _playerObject;
    private InputActions _inputActions;

    public Inventory Inventory => _inventory;
    public string CurrentSceneName
    {
        get { return _currentSceneName; }
        set { _currentSceneName = value; }
    }
    public GameObject PlayerObject
    {
        get { return _playerObject; }
        set { _playerObject = value; }
    }

    public InputActions Actions => _inputActions;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        _currentSceneName = SceneName.TitleScene;
        _inventory = new Inventory(_inventorySlotCount, _inventoryMaxWeight);
        _inputActions = new InputActions();
        _inputActions.Player.Enable();
    }


    public void QuitGame()
    {
        DataManager.Instance.SaveAllData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
