using UnityEngine;

public abstract class Box : MonoBehaviour
{
    private ItemSlot[] _boxSlots;
    protected ItemTypeWeight[] _typeWeights;

    private bool _isOpened = false;

    private readonly int _slotNum = 5;

    private void Awake()
    {
        _boxSlots = new ItemSlot[_slotNum];
        for (int i = 0; i < _slotNum; ++i)
            _boxSlots[i] = new ItemSlot();

        _typeWeights = new ItemTypeWeight[_slotNum];
        for (int i = 0; i < _slotNum; ++i)
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
        if (!_isOpened)
        {
            SetBoxItems();
            _isOpened = true;
        }

        for (int i = 0; i < _slotNum; ++i)
        {
            if (_boxSlots[i].CurrentItem == null)
                UIManager.Instance.ChangeBoxSlotUI(i, 0);
            else
                UIManager.Instance.ChangeBoxSlotUI(i,
                _boxSlots[i].CurrentItem.ID);
        }
    }

    private void SetBoxItems()
    {
        int itemCnt = Random.Range(1, _slotNum + 1);

        for (int i = 0; i < itemCnt; ++i)
        {
            Item item = GetRandomItemByType();
            _boxSlots[i].CurrentItem = item;
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
}
