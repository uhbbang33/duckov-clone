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

    private Player _player;
    private Inventory _inventory;
    private Storage _storage;

    private void Start()
    {
        FieldManager gameManager = FieldManager.Instance;

        GameObject playerObject = gameManager.PlayerObject;
        _storage = gameManager.storage;

        _player = playerObject.GetComponent<Player>();
        _inventory = playerObject.GetComponent<Inventory>();
    }

    #region Save
    public void SavePlayerStats()
    {
        try
        {
            PlayerStatsSaveData data = _player.StatsSaveData;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path.Combine(_savePath, _statsSaveFileName), json);
            Debug.Log("Player Stats 저장 완료");
        }
        catch (Exception ex)
        {
            Debug.Log("Player Stats 저장 실패" + ex.Message);
        }
    }

    public void SavePlayerInventory()
    {
        try
        {
            PlayerInventorySaveData data = _inventory.InventorySaveData;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path.Combine(_savePath, _inventorySaveFileName), json);
            Debug.Log("PlayerInventory 저장 완료");
        }
        catch (Exception ex)
        {
            Debug.Log("PlayerInventory 저장 실패" + ex.Message);
        }
    }

    public void SaveStorage()
    {
        try
        {
            StorageSaveData data = _storage.SaveData;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path.Combine(_savePath, _storageSaveFileName), json);
            Debug.Log("Storage 저장 완료");
        }
        catch (Exception ex)
        {
            Debug.Log("Storage 저장 실패" + ex.Message);
        }
    }

    #endregion Save


    #region Load

    public void LoadPlayerStats()
    {
        string path = Path.Combine(_savePath, _statsSaveFileName);

        if (!File.Exists(path))
        {
            Debug.Log("PlayerStats 데이터 json 파일 없음 - 초기값 사용");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            PlayerStatsSaveData data = JsonUtility.FromJson<PlayerStatsSaveData>(json);
            if (data == null)
            {
                Debug.LogError("PlayerStatsData JSON 파싱 실패");
                return;
            }

            _player.StatsSaveData = data;
        }
        catch (Exception ex)
        {
            Debug.Log("Player Stats 로드 실패" + ex.Message);
        }
    }

    public void LoadPlayerInventory()
    {
        string path = Path.Combine(_savePath, _inventorySaveFileName);

        if (!File.Exists(path))
        {
            Debug.Log("Inventory 데이터 json 파일 없음 - 초기값 사용");
            return;
        }

        //try
        {
            string json = File.ReadAllText(path);
            PlayerInventorySaveData data = JsonUtility.FromJson<PlayerInventorySaveData>(json);
            if (data == null)
            {
                Debug.LogError("PlayerInventoryData JSON 파싱 실패");
                return;
            }

            _inventory.InventorySaveData = data;
        }
        //catch (Exception ex)
        {
          //  Debug.Log("Player Inventory 로드 실패" + ex.Message);
        }
    }

    public void LoadStorage()
    {
        string path = Path.Combine(_savePath, _storageSaveFileName);

        if (!File.Exists(path))
        {
            Debug.Log("Storage 데이터 json 파일 없음 - 초기값 사용");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            StorageSaveData data = JsonUtility.FromJson<StorageSaveData>(json);
            if (data == null)
            {
                Debug.LogError("StorageData JSON 파싱 실패");
                return;
            }

            _storage.SaveData = data;
        }
        catch (Exception ex)
        {
            Debug.Log("Player Inventory 로드 실패" + ex.Message);
        }
    }

    #endregion Load
}
