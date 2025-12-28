using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractableScanner : MonoBehaviour
{
    [SerializeField] private float _showUIDistance;
    [SerializeField] private float _openableDistance;
    [SerializeField] private LayerMask _interactObjectLayer;

    private Collider[] _results;
    private HashSet<InteractableStateUI> _current = new();
    private HashSet<InteractableStateUI> _previous = new();
    private InteractableStateUI _currentNearestUI;
    
    private void Awake()
    {
        _results = new Collider[10];
    }

    private void Start()
    {
        InvokeRepeating(nameof(Check), 0f, 0.1f);
    }

    private void Check()
    {
        _current.Clear();

        int cnt = Physics.OverlapSphereNonAlloc(
            transform.position,
            _showUIDistance,
            _results,
            _interactObjectLayer);

        InteractableStateUI nearestUI = null;
        float minDist = float.MaxValue;

        for (int i = 0; i < cnt; ++i)
        {
            InteractableStateUI ui = _results[i].gameObject.GetComponent<InteractableStateUI>();

            float dist = (transform.position - ui.transform.position).sqrMagnitude;

            if (dist < minDist
                && dist <= _openableDistance * _openableDistance)
            {
                minDist = dist;
                nearestUI = ui;
            }

            _current.Add(ui);

            if (!_previous.Contains(ui))
                ui.ShowCanvas();
        }

        ChangeNearestUI(nearestUI);

        foreach (InteractableStateUI ui in _previous)
        {
            if (!_current.Contains(ui))
                ui.HideCanvas();
        }

        (_previous, _current) = (_current, _previous);
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

}
