
public class LoadingUI : FadeController
{
    private static LoadingUI instance;

    public static LoadingUI Instance
    {
        get { return instance; }
    }

    protected override void Awake()
    {
        base.Awake();

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
