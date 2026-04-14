using UnityEngine;

public class InteractableDoorUI : InteractableStateUI
{
    [SerializeField] private SceneList _targetScene;

    public override void OnInteract()
    {
        base.OnInteract();

        if (_targetScene == SceneList.BUNKER)
        {
            DataManager.Instance.SavePlayerData();
            SceneLoader.Instance.LoadScene(SceneName.BunkerScene);
        }
        else if (_targetScene == SceneList.FIELD)
        {
            DataManager.Instance.SaveAllData();
            SceneLoader.Instance.LoadScene(SceneName.FieldScene);
        }
    }
}
