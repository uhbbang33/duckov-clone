using UnityEngine;

public class GoToAnotherSceneTrigger : MonoBehaviour
{
    [SerializeField] private SceneList _targetScene;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tag.Player))
            return;

        if (_targetScene == SceneList.BUNKER)
            SceneLoader.Instance.LoadScene(SceneName.BunkerScene);
        else if (_targetScene == SceneList.FIELD)
            SceneLoader.Instance.LoadScene(SceneName.FieldScene);
    }
}
