using UnityEngine;

public abstract class Box : MonoBehaviour
{
    private ItemSlot[] _boxSlots;
    protected ItemTypeWeight[] _typeWeights;

    private bool _isOpened;
    private int _slotCnt;

    private void Start()
    {
        _slotCnt = GameManager.Instance.BoxSlotNum;

        _boxSlots = new ItemSlot[_slotCnt];
        for (int i = 0; i < _slotCnt; ++i)
        {
            _boxSlots[i] = new ItemSlot();
            _boxSlots[i].Type = SlotType.BOX;
        }

        _typeWeights = new ItemTypeWeight[_slotCnt];
        for (int i = 0; i < _slotCnt; ++i)
            _typeWeights[i] = new ItemTypeWeight();

        SetWeightValue();

        // TODO : 하드코딩
        _typeWeights[0].Type = ItemType.Gun;
        _typeWeights[1].Type = ItemType.Ammo;
        _typeWeights[2].Type = ItemType.Medicine;
        _typeWeights[3].Type = ItemType.Food;
        _typeWeights[4].Type = ItemType.Etc;
    }

    protected abstract void SetWeightValue();

    public void OpenBox()
    {
        for (int i = 0; i < _slotCnt; ++i)
        {
            _boxSlots[i].UI = GameManager.Instance.BoxItemSlots[i].GetComponent<ItemSlotUI>();
        }

        if (!_isOpened)
        {
            SetBoxItems();
            _isOpened = true;
        }
    }

    private void SetBoxItems()
    {
        int itemCnt = Random.Range(1, _slotCnt + 1);

        for (int i = 0; i < itemCnt; ++i)
        {
            Item item = GetRandomItemByType();
            _boxSlots[i].AddItem(item, 1);
        }
    }

    private Item GetRandomItemByType()
    {
        string type = SetItemType();

        return DataManager.Instance.GetRandomItem(type);
    }

    protected string SetItemType()
    {
        int totalWeightValue = 0;
        foreach (var w in _typeWeights)
            totalWeightValue += w.WeightValue;

        int random = Random.Range(0, totalWeightValue);
        int current = 0;

        foreach (var w in _typeWeights)
        {
            current += w.WeightValue;
            if (random < current)
                return w.Type;
        }

        Debug.Log("weight value random error");
        return ItemType.Etc;
    }

    public void AddItemToEmptySlot(Item item, int amount)
    {
        int slotIndex = FindFirstEmptySlot();

        if (slotIndex == -1)
            return;

        _boxSlots[slotIndex].AddItem(item, amount);
    }

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < _slotCnt; ++i)
        {
            if (_boxSlots[i].CurrentItem == null)
            {
                return i;
            }
        }

        return -1;
    }
}
