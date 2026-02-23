using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _muzzleTransform;

    public Transform MuzzleTransform
    {
        get { return _muzzleTransform; }
    }

}
