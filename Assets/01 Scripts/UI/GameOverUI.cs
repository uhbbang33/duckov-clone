
public class GameOverUI : FadeController
{
    public void ShowGameOverUI()
    {
        StartCoroutine(FadeIn());
    }

    // Button event
    public void OnContinueGame()
    {
        SceneLoader.Instance.LoadScene(SceneName.BunkerScene);
    }
}
