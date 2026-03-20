using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void OnClickNewGame()
    {
        SceneManager.LoadSceneAsync(SceneName.FieldScene);
    }

    public void OnClickLoadGame()
    {

    }

    public void OnClickSettings()
    {

    }

    public void OnClickQuit()
    {

    }

}
