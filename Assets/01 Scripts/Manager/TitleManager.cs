using UnityEngine;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    public void OnClickNewGame()
    {
        Cursor.visible = false;
        SceneLoader.Instance.LoadScene(SceneName.FieldScene);
    }

    public void OnClickLoadGame()
    {
        Cursor.visible = false;
        SceneLoader.Instance.LoadScene(SceneName.FieldScene);
    }

    public void OnClickSettings()
    {

    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
