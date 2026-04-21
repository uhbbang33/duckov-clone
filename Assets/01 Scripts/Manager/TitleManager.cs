using System.IO;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject _newGamePopup;

    private readonly string _saveFilePath = Path.Combine(Application.dataPath, "Resources", "JsonData", "Save");


    private void Start()
    {
        Cursor.visible = true;
    }

    private void LoadBunkerScene()
    {
        SceneLoader.Instance.LoadScene(SceneName.BunkerScene);
    }

    public void OnClickNewGame()
    {
        // 저장된 데이터가 있을 경우 popup
        if (Directory.Exists(_saveFilePath) && Directory.GetFiles(_saveFilePath).Length > 0)
        {
            _newGamePopup.SetActive(true);
        }
        else
        {
            // 없을 경우 그냥 시작
            LoadBunkerScene();
        }
    }

    public void OnClickLoadGame()
    {
        Cursor.visible = false;
        LoadBunkerScene();
    }

    public void OnClickSettings()
    {
        SceneLoader.Instance.LoadScene(SceneName.FieldScene);
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region NewGame button

    public void OnClickConfirmNewGame()
    {
        if (Directory.Exists(_saveFilePath))
        {
            string[] files = Directory.GetFiles(_saveFilePath);

            foreach (string file in files)
            {
                File.Delete(file);
            }

            Debug.Log("모든 파일 삭제 완료");
        }

        LoadBunkerScene();
    }

    public void OnClickCancelNewGame()
    {
        _newGamePopup.SetActive(false);
    }

    #endregion

}
