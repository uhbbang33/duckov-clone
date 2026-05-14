using System;
using System.IO;
using UnityEngine;

public class SaveAndLoadManager : SingletonMonoBehaviour<SaveAndLoadManager>
{
    // TODO : 주소 변경 Application.persistentDataPath
    private readonly string _savePath = Path.Combine("Assets", "Resources", "JsonData", "Save");
    private readonly string _statsSaveFileName = "PlayerStatsSave.json";
    private readonly string _inventorySaveFileName = "PlayerInventorySave.json";
    private readonly string _storageSaveFileName = "StorageSave.json";

    private GameManager _gameManager;
    private Storage _storage;

    private Player _player
    {
        get
        {
            return _gameManager.PlayerObject.GetComponent<Player>();
        }
    }

    private InventoryController _inventoryController
    {
        get
        {
            return _gameManager.PlayerObject.GetComponent<InventoryController>();
        }
    }

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;

        if (BunkerManager.Instance != null)
            _storage = BunkerManager.Instance.storage;
    }

    #region Save
    
    public void Save<T>(T data, string fileName)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path.Combine(_savePath, fileName), json);
        }
        catch (Exception ex)
        {
            Debug.Log(fileName + " 저장 실패" + ex.Message);
        }
    }

    public void SavePlayerStats() => Save(_player.StatsSaveData, _statsSaveFileName);
    public void SavePlayerInventory() => Save(_inventoryController.InventorySaveData, _inventorySaveFileName);
    public void SaveStorage() => Save(_storage.SaveData, _storageSaveFileName);

    public void SaveTime()
    {

    }

    #endregion Save


    #region Load

    public void Load<T>(string fileName, Action<T> onSuccess, Action onFileNotFound = null) where T : class
    {
        string path = Path.Combine(_savePath, fileName);

        if (!File.Exists(path))
        {
            Debug.Log(fileName + " 데이터 json 파일 없음 - 초기값 사용");
            onFileNotFound?.Invoke();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            T data = JsonUtility.FromJson<T>(json);

            if (data == null)
            {
                Debug.LogError(fileName + " JSON 파싱 실패");
                return;
            }

            onSuccess(data);
        }
        catch (Exception ex)
        {
            Debug.Log(fileName + " 로드 실패" + ex.Message);
        }
    }

    public void LoadPlayerStats()
        => Load<PlayerStatsSaveData>(_statsSaveFileName,
        onSuccess: data => _player.StatsSaveData = data);
    
    public void LoadPlayerInventory() => Load<PlayerInventorySaveData>(_inventorySaveFileName,
        onSuccess: data => _inventoryController.InventorySaveData = data,
        onFileNotFound: () => _gameManager.Inventory.ClearInventory());

    public void LoadStorage()
    {
        _storage = BunkerManager.Instance?.storage;

        Load<StorageSaveData>(_storageSaveFileName,
            onSuccess: data => _storage.SaveData = data);
    }

    #endregion Load


    #region Delete

    private void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            //Debug.Log(path + "파일 삭제 완료");
        }
        else
        {
            Debug.Log(path + " 파일이 존재하지 않음");
        }
    }

    public void DeletePlayerStats()
    {
        string path = Path.Combine(_savePath, _statsSaveFileName);

        DeleteFile(path);
    }


    public void DeletePlayerInventory()
    {
        string path = Path.Combine(_savePath, _inventorySaveFileName);

        DeleteFile(path);
    }


    public void DeleteStorage()
    {
        string path = Path.Combine(_savePath, _storageSaveFileName);

        DeleteFile(path);
    }

    #endregion Delete
}
