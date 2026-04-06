using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private string _currentSceneName;

    public string CurrentSceneName
    {
        get { return _currentSceneName; }
        set { _currentSceneName = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        _currentSceneName = SceneName.TitleScene;
    }


    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
