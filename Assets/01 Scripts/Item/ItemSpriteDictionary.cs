using System.Collections.Generic;
using UnityEngine;

public class ItemSpriteDictionary : SingletonMonoBehaviour<ItemSpriteDictionary>
{
    private Dictionary<int, Sprite> _itemSpriteDict;

    protected override void Awake()
    {
        base.Awake();
        CreateItemDictionary();
    }

    private void CreateItemDictionary()
    {
        _itemSpriteDict = new();

        foreach (var sprite in GameResources.Instance.ItemSprites)
        {
            string[] split = sprite.name.Split('_');
            int id = int.Parse(split[1]);

            _itemSpriteDict[id] = sprite;
        }
    }

    public Sprite GetItemSprite(uint id)
    {
        if(_itemSpriteDict.TryGetValue((int)id, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogError("Item sprite not found id : " + id);

        return null;
    }
}
