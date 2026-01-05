using UnityEngine;

public abstract class Box : MonoBehaviour
{
    private bool _isOpened = false;
    protected Item[] _items;
    protected ItemTypeWeight[] _typeWeights = new ItemTypeWeight[5];

    private void Awake()
    {
        _items = new Item[5];

        for (int i = 0; i < 5; ++i)
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
    }

    private void SetBoxItems()
    {
        int itemCnt = Random.Range(1, _items.Length);

        for (int i = 0; i < itemCnt; ++i)
        {
            _items[i] = GetRandomItemByType();

            UIManager.Instance.ChangeBoxSlotUI(i, _items[i].ID);
        }
        // TODO
        for(int i= itemCnt; i < 5; ++i)
        {
            _items[i] = null;
            UIManager.Instance.ChangeBoxSlotUI(i, 0);
        }
    }

    private Item GetRandomItemByType()
    {
        ItemType type = SetItemType();

        return DataManager.Instance.GetRandomItem(type);
    }

    protected ItemType SetItemType()
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
