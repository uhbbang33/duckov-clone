using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private GameObject _renderersParent;

    public Transform MuzzleTransform
    {
        get { return _muzzleTransform; }
    }

    public void SetRendererEnabled(bool isEnabled)
    {
        _renderersParent.SetActive(isEnabled);
    }
}
