using UnityEngine;

public class Storage : MonoBehaviour
{
    private ItemSlot[] _slots;
    private StorageUI _ui;

    private GameManager _gameManager;

    private int _slotCnt;

    private void Awake()
    {
        _ui = GetComponent<StorageUI>();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _slotCnt = _gameManager.StorageItemSlots.Length;

        _slots = new ItemSlot[_slotCnt];
        for(int i =0; i < _slotCnt; ++i)
        {
            _slots[i] = new ItemSlot();
            _slots[i].UI = _gameManager.StorageItemSlots[i].GetComponentInChildren<ItemSlotUI>();
            _slots[i].Type = SlotType.STORAGE;
        }
    }

    
}
