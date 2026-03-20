using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    public void OnClickNewGame()
    {
        Cursor.visible = false;
        SceneManager.LoadSceneAsync(SceneName.FieldScene);
    }

    public void OnClickLoadGame()
    {
        Cursor.visible = false;
        SceneManager.LoadSceneAsync(SceneName.FieldScene);
    }

    public void OnClickSettings()
    {

    }

    public void OnClickQuit()
    {

    }

}
