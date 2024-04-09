using System;
using DG.Tweening;
using UnityEngine;

public class PlayerNextStageController : MonoBehaviour
{
    public static PlayerNextStageController Instance;
    [SerializeField] private Transform targetLocationWhenLevelUp;

    private Rigidbody2D rigidbody2D;
    
    private void Awake()
    {
        Instance = this;
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void LaunchToNextStage(Action callback)
    {
        transform.DOMoveY(targetLocationWhenLevelUp.position.y, 1f).OnComplete(() =>
        {
            rigidbody2D.constraints = (RigidbodyConstraints2D)RigidbodyConstraints.FreezePosition; 
            callback?.Invoke();
        });
    }
}
