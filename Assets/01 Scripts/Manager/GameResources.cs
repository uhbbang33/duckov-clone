using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region SPRITES
    [Space(10)]
    [Header("SPRITES")]
    public Sprite InteractableSprite;
    public Sprite OpenedBoxSprite;
    public Sprite OpenableBoxSprite;
    #endregion

    #region ITEMS
    [Space(10)]
    [Header("ITEMS")]
    public Sprite[] ItemSprites;
    #endregion

    #region GUNS
    [Space(10)]
    [Header("GUNS")]
    public GameObject Mp7Prefab;
    public GameObject M700Prefab;
    public GameObject GlockPrefab;
    #endregion

    #region BOXS
    [Space(10)]
    [Header("BOXS")]
    public GameObject LootBoxPrefab;
    #endregion
}
