using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHazard : PlatformTimer
{
    [SerializeField] private Material _hazardMaterial;
    [SerializeField] private float _hazardTime = 3;
    private float _hazardTimer;

    private Material _normalMaterial;
    private SpriteRenderer _sr;
    private Collider2D _collider;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _normalMaterial = _sr.material;
    }

    protected override void Update()
    {
        if (TimerStart || PlayerController.Instance.PlatCol == _collider) return;

        Timer += Time.deltaTime;
        if (Timer >= LifeTime)
        {
            _sr.material = _hazardMaterial;
            _hazardTimer += Time.deltaTime;
            if (_hazardTimer >= _hazardTime)
            {
                ResetHazard();
            }
        }
    }

    private void ResetHazard()
    {
        _sr.material = _normalMaterial;
        _hazardTimer = 0;
        Timer = 0;
    }

    public override bool StartTimer()
    {
        if (Timer >= LifeTime)
        {
            //Kill off player
            //PlayerController.Instance.DisableBodyCollider();
            //PlayerController.Instance.DisableLaunchCapabilities();
            ResetHazard();
            return false;
        }
        return base.StartTimer();
    }
}
