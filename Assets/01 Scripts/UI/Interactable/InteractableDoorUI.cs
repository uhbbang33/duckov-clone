using UnityEngine;

public class InteractableDoorUI : InteractableStateUI
{
    [SerializeField] private SceneList _targetScene;

    public override void OnInteract()
    {
        base.OnInteract();

        if (_targetScene == SceneList.BUNKER)
            SceneLoader.Instance.LoadScene(SceneName.BunkerScene);
        else if (_targetScene == SceneList.FIELD)
            SceneLoader.Instance.LoadScene(SceneName.FieldScene);
    }
}
