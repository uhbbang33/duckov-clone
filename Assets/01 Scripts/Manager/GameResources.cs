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
    public Sprite UnopenedBoxSprite;
    public Sprite OpenedBoxSprite;
    public Sprite OpenableBoxSprite;
    #endregion

}
