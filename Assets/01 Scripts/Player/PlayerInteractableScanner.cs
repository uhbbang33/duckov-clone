using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractableScanner : MonoBehaviour
{
    [SerializeField] private float _showUIDistance;
    [SerializeField] private float _openableDistance;
    [SerializeField] private LayerMask _interactObjectLayer;
    [SerializeField] private float _nearestSphereZOffset;
    [SerializeField] private float _nearestSphereYOffset;

    private Collider[] _farResults;
    private Collider[] _nearResults;
    private HashSet<InteractableStateUI> _current = new();
    private HashSet<InteractableStateUI> _previous = new();
    private InteractableStateUI _currentNearestUI;
    
    private void Awake()
    {
        _farResults = new Collider[10];
        _nearResults = new Collider[10];
    }

    private void Start()
    {
        StartCheck();
    }

    private void Check()
    {
        ShowInteractableUI();

        GameObject nearestObj = FindNearestObject();

        if (nearestObj != null)
        {
            ChangeNearestObject(nearestObj);
            ChangeNearestUI(nearestObj.GetComponent<InteractableStateUI>());
        }
        else
        {
            ChangeNearestUI(null);
        }
    }

    private void ShowInteractableUI()
    {
        _current.Clear();

        int farBoxCnt = Physics.OverlapSphereNonAlloc(
            transform.position,
            _showUIDistance,
            _farResults,
            _interactObjectLayer);

        for (int i = 0; i < farBoxCnt; ++i)
        {
            GameObject obj = _farResults[i].gameObject;
            InteractableStateUI ui = obj.GetComponent<InteractableStateUI>();

            _current.Add(ui);

            if (!_previous.Contains(ui))
                ui.ShowCanvas();
        }

        foreach (InteractableStateUI ui in _previous)
        {
            if (!_current.Contains(ui) && ui != null)
                ui.HideCanvas();
        }

        (_previous, _current) = (_current, _previous);
    }

    private GameObject FindNearestObject()
    {
        GameObject nearestObj = null;
        float minDist = float.MaxValue;

        Vector3 scanPos = transform.position + transform.forward * _nearestSphereZOffset + transform.up * _nearestSphereYOffset;

        int nearBoxCnt  = Physics.OverlapSphereNonAlloc(
                 scanPos,
                _openableDistance,
                _nearResults,
                _interactObjectLayer);

        for (int i = 0; i < nearBoxCnt; ++i)
        {
            GameObject obj = _nearResults[i].gameObject;

            float dist = (scanPos - obj.transform.position).sqrMagnitude;

            if (dist < minDist)
            {
                dist = minDist;
                nearestObj = obj;
            }
        }

        return nearestObj;
    }

    private void ChangeNearestUI(InteractableStateUI nearestUI)
    {
        if (_currentNearestUI == nearestUI)
            return;

        if (_currentNearestUI != null)
            _currentNearestUI.Deselected();

        _currentNearestUI = nearestUI;

        if (_currentNearestUI != null)
            _currentNearestUI.Selected();
    }

    private void ChangeNearestObject(GameObject obj)
    {
        if (obj == null)
            return;

        GameManager.Instance.CurrentBox = obj.GetComponent<Box>();
    }

    public void StartCheck()
    {
        InvokeRepeating(nameof(Check), 0f, 0.1f);
    }

    public void HideAllInteractUI()
    {
        CancelInvoke(nameof(Check));

        _previous.Clear();
        foreach (InteractableStateUI ui in _current)
        {
            ui.HideCanvas();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * _nearestSphereZOffset + transform.up * _nearestSphereYOffset, _openableDistance);
    }
}
