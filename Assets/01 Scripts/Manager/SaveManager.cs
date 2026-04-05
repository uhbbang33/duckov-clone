using System;
using System.IO;
using UnityEngine;

public class SaveManager : SingletonMonoBehaviour<SaveManager>
{
    // TODO : 주소 변경 Application.persistentDataPath
    private readonly string _savePath = Path.Combine("Assets", "Resources", "JsonData", "Save");
    private readonly string _statsSaveFileName = "PlayerStatsSave.json";
    private readonly string _inventorySaveFileName = "PlayerInventorySave.json";
    private readonly string _storageSaveFileName = "StorageSave.json";

    private QuickSlotManager _quickSlotManager;
    private HealthPoint _playerHP;
    private StaminaPoint _playerSP;
    private Hydration _hydration;
    private Hunger _hunger;
    private PlayerEquip _playerEquip;
    private Inventory _inventory;
    private Storage _storage;

    private void Start()
    {
        _quickSlotManager = QuickSlotManager.Instance;

        GameManager gameManager = GameManager.Instance;

        GameObject playerObject = gameManager.PlayerObject;
        _storage = gameManager.storage;

        _playerHP = playerObject.GetComponent<HealthPoint>();
        _playerSP = playerObject.GetComponent<StaminaPoint>();
        _hydration = playerObject.GetComponent<Hydration>();
        _hunger = playerObject.GetComponent<Hunger>();
        _playerEquip = playerObject.GetComponent<PlayerEquip>();
        _inventory = playerObject.GetComponent<Inventory>();
    }

    public void SavePlayerStats()
    {
        try
        {
            PlayerStatsSaveData data = new PlayerStatsSaveData
            {
                CurrentHP = _playerHP.CurrentHP,
                CurrentSP = Mathf.Floor(_playerSP.CurrentSP),
                CurrentHydration = Mathf.Floor(_hydration.Current),
                CurrentHunger = Mathf.Floor(_hunger.Current)
            };

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
            PlayerInventorySaveData data = new PlayerInventorySaveData
            {
                EquipedGunSlotNum = _playerEquip.EquipNum,
                QuickSlotLinkedInventoryIndex = _quickSlotManager.GetLinkedInventoryIndexList(),

                ItemIDList = SaveUtility.GetSlotItemsID(_inventory),
                GunItemAmmoCountList = SaveUtility.GetSlotGunItemsAmmoCount(_inventory),
                QuantityList = SaveUtility.GetSlotsQuantity(_inventory),
                DurabilityList = SaveUtility.GetSlotItemsDurability(_inventory),

                Money = _inventory.CurrnetMoney
            };

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
            StorageSaveData data = new StorageSaveData
            {
                ItemIDList = SaveUtility.GetSlotItemsID(_storage),
                GunItemAmmoCountList = SaveUtility.GetSlotGunItemsAmmoCount(_storage),
                QuantityList = SaveUtility.GetSlotsQuantity(_storage),
                DurabilityList = SaveUtility.GetSlotItemsDurability(_storage),
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path.Combine(_savePath, _storageSaveFileName), json);
            Debug.Log("Storage 저장 완료");
        }
        catch (Exception ex)
        {
            Debug.Log("Storage 저장 실패" + ex.Message);
        }
    }
}
