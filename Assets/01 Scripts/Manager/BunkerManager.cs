using UnityEngine;

public class BunkerManager : SingletonMonoBehaviour<BunkerManager>
{
    [SerializeField] private GameObject[] _storageItemSlots;
    [SerializeField] private GameObject[] _shopItemSlots;

    [SerializeField] private Storage _storage;
    [SerializeField] private GameObject _playerObject;

    [SerializeField] private AudioSource _audioSource;

    private GameManager _gameManager;

    private bool _isStorageOpened;
    private bool _isShopOpened;

    public GameObject[] StorageItemSlots { get { return _storageItemSlots; } }
    public GameObject[] ShopItemSlots { get { return _shopItemSlots; } }
    public Storage storage { get { return _storage; } }

    public bool IsStorageOpened
    {
        get { return _isStorageOpened; }
        set { _isStorageOpened = value; }
    }

    public bool IsShopOpened
    {
        get { return _isShopOpened; }
        set { _isShopOpened = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        _gameManager = GameManager.Instance;
        _gameManager.PlayerObject = _playerObject;
    }

    private void Start()
    {
        SoundManager.Instance.PlayBunkerBGM(_audioSource);
        _audioSource.ignoreListenerPause = true;
    }
}
