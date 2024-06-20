using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SkipLevelController : MonoBehaviour
{
    [SerializeField] private Transform playerController;
    [SerializeField] private Button skipButton;

    private void Awake()
    {
        skipButton.onClick.AddListener(OnClickSkipButton);
    }

    private void OnClickSkipButton()
    {
        var position = LevelManager.Instance.CurrentTargetPlatform.transform.position;
        position = new Vector2(position.x - 0.5f, position.y + 2f);
        playerController.DOMove(position, 0f);
    }
}
