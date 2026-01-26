using System.Collections;
using UnityEngine;

public abstract class Box : MonoBehaviour
{
    private ItemSlot[] _boxSlots;
    private BoxSlotLoad[] _boxSlotLoad;
    protected ItemTypeWeight[] _typeWeights;
    private InteractableBoxUI _boxInteractableUI;

    private int _slotCnt;
    private int _itemCnt;

    private bool _allRarityOpened;

    //TODO Define으로
    private const int _ammoQuantity = 30;

    private void Awake()
    {
        _boxInteractableUI = GetComponent<InteractableBoxUI>();
    }

    private void Start()
    {
        _slotCnt = GameManager.Instance.BoxSlotNum;

        _boxSlots = new ItemSlot[_slotCnt];
        for (int i = 0; i < _slotCnt; ++i)
        {
            _boxSlots[i] = new ItemSlot();
            _boxSlots[i].Type = SlotType.BOX;
        }

        _boxSlotLoad = new BoxSlotLoad[_slotCnt];

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
        GameManager.Instance.CurrentOpenBox = this;

        for (int i = 0; i < _slotCnt; ++i)
        {
            _boxSlots[i].UI = GameManager.Instance.BoxItemSlots[i].GetComponentInChildren<ItemSlotUI>();

            _boxSlotLoad[i] = GameManager.Instance.BoxItemSlots[i].GetComponent<BoxSlotLoad>();
        }

        if (!_boxInteractableUI.HasBeenOpened)
        {
            SetBoxItems();
            _boxInteractableUI.HasBeenOpened = true;
        }

        if (!_allRarityOpened)
        {
            StartCoroutine(SlotLoadRoutine());
        }
    }

    private void SetBoxItems()
    {
        int itemCnt = Random.Range(1, _slotCnt + 1);

        for (int i = 0; i < itemCnt; ++i)
        {
            Item item = GetRandomItemByType();

            int itemQuantity = 1;
            if (item.Type == ItemType.Ammo)
                itemQuantity = _ammoQuantity;

            _boxSlots[i].AddItem(item, itemQuantity);
        }

        _itemCnt = itemCnt;
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

    public void ChangeBoxItemCount(bool isAdd)
    {
        _itemCnt += isAdd ? 1 : -1;
        UIManager.Instance.ChangeBoxItemCountText(_itemCnt, _slotCnt);
    }

    private IEnumerator SlotLoadRoutine()
    {
        for (int i = 0; i < _itemCnt; ++i)
            _boxSlotLoad[i].SetItemSlotBeforeLoad();

        for (int i = _itemCnt; i < _slotCnt; ++i)
            _boxSlotLoad[i].SetEmptySlot();

        for (int i = 0; i < _itemCnt; ++i)
        {
            _boxSlotLoad[i].StartLoad();

            float loadingTime = SetLoadingTime(_boxSlots[i].CurrentItem.Rarity);
            yield return new WaitForSeconds(loadingTime);

            _boxSlotLoad[i].LoadComplete();
        }

        _allRarityOpened = true;

        yield return null;
    }

    private float SetLoadingTime(string rarity)
    {
        if (rarity == "일반")
            return RarityLoadingTime.Common;
        else if (rarity == "고급")
            return RarityLoadingTime.Uncommon;
        else if (rarity == "희귀")
            return RarityLoadingTime.Rare;
        else if (rarity == "전설")
            return RarityLoadingTime.Legendary;

        Debug.Log("해당안됨 " + rarity);

        return 0;
    }
}
