using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Platform))]
public class PlatformTimer : MonoBehaviour
{
    [SerializeField] private float _lifetime = 5;
    protected float LifeTime => _lifetime;

    protected float Timer { get; set; }
    protected bool TimerStart { get; private set; }

    protected virtual void Update()
    {
        if (!TimerStart) return;

        Timer += Time.deltaTime;
        if (Timer >= _lifetime)
        {
            //Kill off player
            //PlayerController.Instance.DisableBodyCollider();
            //PlayerController.Instance.DisableLaunchCapabilities();
            
            gameObject.SetActive(false);
        }
    }

    public virtual bool StartTimer()
    {
        TimerStart = true;
        return true;
    }

    public void StopTimer()
    {
        TimerStart = false;
        Timer = 0;
    }
}
