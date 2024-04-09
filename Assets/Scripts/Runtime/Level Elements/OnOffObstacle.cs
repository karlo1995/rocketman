using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffObstacle : MonoBehaviour
{
    [SerializeField] private float _maxActiveTime;
    private float _activeTime;
    private bool _isActive;

    private Collider2D _col2D;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _col2D = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        TurnOn(true);
    }

    private void Update()
    {
        _activeTime -= Time.deltaTime;

        if (_activeTime < 0)
        {
            _activeTime = _maxActiveTime;
            TurnOn(!_isActive);
        }
    }

    public void TurnOn(bool isOn)
    {
        _col2D.enabled = isOn;
        _sr.enabled = isOn;
        _isActive = isOn;
    }
}
